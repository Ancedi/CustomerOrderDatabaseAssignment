# CustomerOrderDatabaseAssignment

Notera: View har inte decrypterar inte säkrad information.

När programmet startar presenteras användaren med en meny som består av 4 numrerade val.
Customer, Order, Product, och Exit.

Steg 1:
- Välj en av de 4 kategorierna med dess respektiva nummer. 1 för Customer och så vidare.
- Efter att användaren har valt en av 4 valen, med undantag för exit som avbryter programmet, tas de vidare till en ytterligare meny.

Steg 2:
- Customer, Order, och Product har alla CRUD funktioner. Användaren kan alltså välja att skapa, radera, editera, och visa innehåll som tillhör de respektiva kategorier.
- Order har en mer utvecklad meny val, specifikt inom visningen av ordrar. Användaren kan välja att bl.a. se vilka produkter som köpts för vilken order eller endast se datum, total kostnad, och köpare (inklusive små detaljer som ID).

Steg 3:
- Skapa, radera, editera, eller visa. Användaren promptas hos alla till att välja en kund, order, eller produkt beroende på vilken kategori som detta sker i.
- Eftersom databasen är tom, besvaras användaren med "Empty" eller liknande vid användning av funktioner utom Skapa tills användaren tillägger innehåll.
- Order skiljer sig från de andra med ett säkerhetskrav om en försöker lägga till en order. Användaren måste ange sitt lösenord för att konfirmera och lägga till ordern.
