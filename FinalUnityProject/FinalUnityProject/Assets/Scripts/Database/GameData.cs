
public class GameData
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int ClearTime { get; set; }
    public int Friendly { get; set; }
    public string Date { get; set; }

    public GameData()
    {
        Id = 0;
        Name = "";
        ClearTime = 0;
        Friendly = 0;
        Date = "";
    }
}
