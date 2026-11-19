// See https://aka.ms/new-console-template for more information

using HtmlAgilityPack;

var testUrl = "https://docs.google.com/document/d/e/2PACX-1vSvM5gDlNvt7npYHhp_XfsJvuntUhq184By5xO_pA4b_gCWeXb6dM6ZxwN8rE6S4ghUsCj2VKR21oEP/pub";
await ReadDoc(testUrl);
return;

async Task ReadDoc(string url)
{
    using var client = new HttpClient();
    var html = await client.GetStringAsync(url);

    var grid = new List<List<string>>();

    var doc = new HtmlDocument();
    doc.LoadHtml(html);
    
    var table = doc.DocumentNode.SelectSingleNode("//table");
    foreach (var row in table?.SelectNodes(".//tr")!)
    {
        var list = new List<string>();
        foreach (var cell in row.SelectNodes(".//td")!)
        {
            list.Add(cell.InnerText);
        }
        grid.Add(list);
    }

    //remove headers from parsed table
    grid.RemoveAt(0);
    
    //find size of result array
    var columns = grid.Max(row => int.Parse(row[0]) + 1);
    var rows = grid.Max(row => int.Parse(row[2]) + 1);

    //initialize result array and fill with spaces
    var result = new string[rows, columns];
    for (var r = 0; r < rows; r++)
    {
        for (var c = 0; c < columns; c++)
        {
            result[r, c] = " ";
        }
    }
    
    //replaces coordinates with specified character from table
    foreach (var row in grid)
    {
        var x = int.Parse(row[0]);
        var y = int.Parse(row[2]);
        var value = row[1];
        
        result[y, x] = value;
    }
    
    //print
    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < columns; j++)
        {
            Console.Write(result[i, j] + "");
        }
        Console.WriteLine();
    }
}