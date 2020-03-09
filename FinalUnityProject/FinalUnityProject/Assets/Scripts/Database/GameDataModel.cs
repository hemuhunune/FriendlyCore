using System.Collections;
using System.Collections.Generic;
using MiniJSON;
public class GameDataModel
{
    public static List<GameData> DeserializeFromJson(string sStrJson)
    {
        var ret = new List<GameData>();

        // JSONデータは最初は配列から始まるので、Deserialize（デコード）した直後にリストへキャスト      
        IList jsonList = (IList)Json.Deserialize(sStrJson);

        // リストの内容はオブジェクトなので、辞書型の変数に一つ一つ代入しながら、処理
        foreach (IDictionary jsonOne in jsonList)
        {
            var tmp = new GameData();

			if (jsonOne.Contains("Id"))
			{
				tmp.Id = (int)(long)jsonOne["Id"];
			}
			if (jsonOne.Contains("name"))
			{
				tmp.Name = (string)jsonOne["name"];
			}
			if (jsonOne.Contains("cleartime"))
			{
				tmp.ClearTime = (int)(long)jsonOne["cleartime"];
			}
			if (jsonOne.Contains("friendly"))
			{
				tmp.Friendly = (int)(long)jsonOne["friendly"];
			}
			if (jsonOne.Contains("Date"))
			{
				tmp.Date = (string)jsonOne["Date"];
			}
			//現レコード解析終了
			ret.Add(tmp);

		}

            return ret;
    }
}
