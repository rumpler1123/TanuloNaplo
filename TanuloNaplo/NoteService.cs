using Microsoft.EntityFrameworkCore;
using OpenAI.Chat;

namespace TanuloNaplo;

public class NoteService
{
    private readonly NaploContext _context;
    private readonly string _openAiApiKey;

    public NoteService(NaploContext context, IConfiguration configuration)
    {
        _context = context;
        _context.Database.EnsureCreated();

        _openAiApiKey = configuration["OpenAI:ApiKey"]
                        ?? throw new InvalidOperationException("Hiányzik az OpenAI:ApiKey beállítás (UserSecrets).");
    }

    public async Task<List<UserNote>> GetNotesAsync(string userId)
    {
        return await _context.Notes
                             .Where(n => n.UserId == userId)
                             .OrderByDescending(n => n.CreatedAt)
                             .ToListAsync();
    }

    // AI ÖSSZEFOGLALÓ
    public async Task<string> GenerateAiSummaryAsync(string userId)
    {
        // jegyzetek lekérése
        var now = DateTime.Now;
        var notes = await _context.Notes
            .Where(n => n.UserId == userId
                        && n.CreatedAt.Year == now.Year
                        && n.CreatedAt.Month == now.Month)
            .OrderBy(n => n.CreatedAt)
            .ToListAsync();

        if (!notes.Any())
            return "Ebben a hónapban nincs jegyzet, nincs mit összefoglalni.";

        // jegyzetek összefűzése promptba
        string collectedNotes = string.Join(
            "\n\n---\n\n",
            notes.Select(n =>
                $"{n.CreatedAt:yyyy-MM-dd} • [{n.CourseName}]\n{n.NoteContent}"
            )
        );

        string systemMessage =
            "Te egy professzionális, magyar nyelvű tanulmányi mentor asszisztens vagy. " +
            "Feladatod: egyetemi hallgatók havi tanulmányi előrehaladásáról készíteni jól tagolt, " +
            "könnyen olvasható, motiváló és szakmailag precíz összefoglalókat." +
            "\n\nFontos elvárások:" +
            "\n- Használj strukturált szekciókat és alcímeket." +
            "\n- Adj bulletpontokat a főbb témákhoz." +
            "\n- Adj visszajelzést és pozitív megerősítést." +
            "\n- Maradj tömör, de informatív." +
            "\n- Formázz Markdown-ban (## címek, **kiemelés**, - pontok)." +
            "\n- Ne használj túl hosszú bekezdéseket.";

        string userPrompt =
            $"Készíts egy esztétikusan tagolt havi tanulmányi riportot a következő jegyzetekből.\n" +
            $"A riport tartalmazzon:\n" +
            $"- rövid, motiváló bevezetőt,\n" +
            $"- főbb tanult témák listáját,\n" +
            $"- készségfejlődési pontokat,\n" +
            $"- egy összegző ajánlást a következő hónapra.\n\n" +
            $"Felhasználó: {userId}\n" +
            $"Időszak: {now.Year}. {now.Month}. hónap\n\n" +
            $"### Jegyzetek:\n{collectedNotes}";

        //  ChatClient létrehozása
        var client = new ChatClient(
            model: "gpt-4.1-mini",
            apiKey: _openAiApiKey
        );

        // OpenAI hívás
        try
        {
            ChatCompletion result = await client.CompleteChatAsync(
                new ChatMessage[]
                {
                new SystemChatMessage(systemMessage),
                new UserChatMessage(userPrompt)
                }
            );

            return result.Content[0].Text;
        }
        catch (System.ClientModel.ClientResultException)
        {
            // ide jön a 429 is
            return "Nem sikerült az AI-összefoglaló: nincs elegendő OpenAI kvóta vagy hibás a számlázás. " +
                   "Kérjük, ellenőrizd az OpenAI fiókod beállításait.";
        }
        catch (Exception)
        {
            return "Váratlan hiba történt az AI-összefoglaló generálása közben.";
        }
    }

    // LÉTREHOZÁS
    public async Task AddNoteAsync(UserNote note)
    {
        note.CreatedAt = DateTime.Now;
        _context.Notes.Add(note);
        await _context.SaveChangesAsync();
    }

    // FRISSÍTÉS
    public async Task UpdateNoteAsync(UserNote note)
    {
        var existingNote = await _context.Notes.FindAsync(note.Id);
        if (existingNote != null)
        {
            existingNote.CourseName = note.CourseName;
            existingNote.NoteContent = note.NoteContent;
            existingNote.UserId = note.UserId;
            await _context.SaveChangesAsync();
        }
    }

    // TÖRLÉS
    public async Task DeleteNoteAsync(int id)
    {
        var note = await _context.Notes.FindAsync(id);
        if (note != null)
        {
            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
        }
    }
}