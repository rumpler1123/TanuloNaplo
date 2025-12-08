using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TanuloNaplo;

public class NoteService
{
    private readonly NaploContext _context;

    public NoteService(NaploContext context)
    {
        _context = context;
        _context.Database.EnsureCreated();
    }

    // 1. OLVASÁS - Most már kér UserID-t! (Ez oldja meg a piros GetNotesAsync hibát)
    public async Task<List<UserNote>> GetNotesAsync(string userId)
    {
        return await _context.Notes
                             .Where(n => n.UserId == userId)
                             .OrderByDescending(n => n.CreatedAt)
                             .ToListAsync();
    }

    // 2. AI ÖSSZEFOGLALÓ (Ez oldja meg a piros GenerateAiSummaryAsync hibát)
    public async Task<string> GenerateAiSummaryAsync(string userId)
    {
        var notes = await _context.Notes
            .Where(n => n.UserId == userId)
            .ToListAsync();

        if (!notes.Any()) return "Nincs elegendő adat az elemzéshez.";

        // AI Szimuláció (Prompt Design bemutatása)
        string systemMessage = @"Te egy többnyelven beszélő, segítőkész asszisztens vagy, aki oktatóként működik! 
        Neved: Edu! Feladatod, hogy tanítsd a felhasználókat.";

        await Task.Delay(2000); // Gondolkodás imitálása

        var kurzusok = notes.Select(n => n.CourseName).Distinct();
        return $"🤖 Edu: Szia {userId}! Ebben a hónapban a következő tárgyakkal foglalkoztál: {string.Join(", ", kurzusok)}. " +
               $"Összesen {notes.Count} jegyzetet készítettél. Csak így tovább!";
    }

    // 3. LÉTREHOZÁS
    public async Task AddNoteAsync(UserNote note)
    {
        note.CreatedAt = DateTime.Now;
        _context.Notes.Add(note);
        await _context.SaveChangesAsync();
    }

    // 4. FRISSÍTÉS
    public async Task UpdateNoteAsync(UserNote note)
    {
        var existingNote = await _context.Notes.FindAsync(note.Id);
        if (existingNote != null)
        {
            existingNote.CourseName = note.CourseName;
            existingNote.NoteContent = note.NoteContent;
            existingNote.UserId = note.UserId; // Biztosítjuk, hogy a user is frissüljön
            await _context.SaveChangesAsync();
        }
    }

    // 5. TÖRLÉS
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