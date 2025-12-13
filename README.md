# TanuloNaplo

Egyszerû tanulmányi napló alkalmazás jegyzetek kezelésére, hangvezérléssel és opcionális AI-alapú havi összefoglalóval.

## Funkciók
- Jegyzetek létrehozása, szerkesztése, törlése
- Tantárgyankénti jegyzetelés
- Hangvezérelt jegyzetbevitel (speech-to-text)
- AI-alapú havi tanulmányi összefoglaló (OpenAI)

---

## Követelmények
- .NET SDK 10
- Visual Studio
- Internetkapcsolat (AI funkcióhoz)

---

## Projekt futtatása
1. Klónozd a repót
2. Nyisd meg a `TanuloNaplo` projektet Visual Studio-ban
3. Futtasd az alkalmazást (`Run` / `F5`)

---

## OpenAI AI-összefoglaló beállítása (opcionális)

> Ha nincs OpenAI kulcs beállítva, az alkalmazás ettõl még mûködik,  
> csak az AI összefoglaló funkció nem lesz elérhetõ.

### 1. Lépés: OpenAI API kulcs generálása
- Lépj be az OpenAI fiókodba:
  https://platform.openai.com/settings/organization/api-keys
- Hozz létre egy új API kulcsot
- Másold ki a kulcsot

### 2. Lépés: User Secrets megnyitása
Visual Studio-ban:
- Jobb klikk a projekten (`TanuloNaplo`)
- **Manage User Secrets**
- Megnyílik egy `secrets.json` fájl

### 3. Lépés: Kulcs beírása
A `secrets.json` tartalma legyen:

```json
{
  "OpenAI:ApiKey": "API_KULCSOD"
}
