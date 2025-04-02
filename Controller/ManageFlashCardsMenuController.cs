using System.Threading.Tasks;
using Spectre.Console;

class ManageFlashCardsMenuController
{
    static Stack currentStack = default;
    static LinkedList<FlashcardDTO> flashcards = [];
    public static async Task Start()
    {
        bool exit = false;

        Console.Clear();
        List<Stack> stackSet = await DataBaseManager<Stack>.GetLogs();
        currentStack = GetInput.Selection(stackSet);
        while (!exit)
        {
            flashcards = await GetCardsAsLinkedList();
            Console.Clear();

            exit = await HandleUserInput();

            if (!exit)
            {
                AnsiConsole.Markup("[bold green]Press Enter to continue. [/]");
                Console.Read();
            }

        }
    }
    
    private static async Task<LinkedList<FlashcardDTO>> GetCardsAsLinkedList()
    {
        LinkedList<FlashcardDTO> flashcards = [];
        string query = "WHERE Stacks_Id = " + currentStack.Id;
        List<Flashcard> flashCardSet = await DataBaseManager<Flashcard>.GetLogs(query);

        foreach (var card in flashCardSet)
        {
            flashcards.AddLast(new FlashcardDTO(card));
        }

        return flashcards;
    }

    private static void ViewCards(int amount = 100)
    {
        List<FlashcardDTO> flashcardDTOs = [];

        if (flashcards.Count != 0)
            for (LinkedListNode<FlashcardDTO> current = flashcards.First;
                current != null && (current.Value.Id < amount);
                current = current.Next)
            {
                flashcardDTOs.Add(current.Value);
            }

        DisplayData.Table(flashcardDTOs, currentStack.Name);
    }

    private static async Task CreateCard()
    {        
        GetInput.FlashcardSides(out string front, out string back);
        
        await DataBaseManager<Flashcard>.InsertLog([
            currentStack.Id.ToString(),
            (flashcards.Count + 1).ToString(),
            "'" + front + "'",
            "'" + back + "'"
        ]);
    }

    private static void EditCard()
    {
        throw new NotImplementedException();
    }

    private static void DeleteCard()
    {
        throw new NotImplementedException();
    }


    private static async Task<bool> HandleUserInput()
    {
        Enums.ManageFlashCardsMenuOptions userInput = DisplayMenu.ManageFlashCardsMenu();
        switch (userInput)
        {
            case Enums.ManageFlashCardsMenuOptions.VIEWALLCARDS:
                ViewCards();
                break;
            case Enums.ManageFlashCardsMenuOptions.VIEWXCARDS:
                ViewCards(10);
                break;
            case Enums.ManageFlashCardsMenuOptions.CREATECARD:
                await CreateCard();
                break;
            case Enums.ManageFlashCardsMenuOptions.EDITCARD:
                EditCard();
                break;
            case Enums.ManageFlashCardsMenuOptions.DELETECARD:
                DeleteCard();
                break;
            case Enums.ManageFlashCardsMenuOptions.BACK:
                return true;
        }

        return false;
    }
}