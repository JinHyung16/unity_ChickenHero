using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class CSVReader : MonoBehaviour
{
    static string splite_read = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string splite_read_line = @"\r\n|\n\r|\n|\r";
    static char[] TRIM_CHARS = { '\"' };

    public static List<Dictionary<string, object>> Read(string file)
    {
        var list = new List<Dictionary<string, object>>();
        TextAsset data = Resources.Load(file) as TextAsset;

        var lines = Regex.Split(data.text, splite_read_line); // CSV파일의 작성되어 있는 모든 라인을 읽어 라인수를 저장한다.

        if (lines.Length <= 1)
        {
            return list;
        }

        var header_colum = Regex.Split(lines[0], splite_read); // 맨 위 첫 줄을 읽는다.

        for (var i = 1; i < lines.Length; i++) // header를 제외한 다음 줄부터 라인을 한줄씩 다 읽는다.
        {
            var values = Regex.Split(lines[i], splite_read);
            if (values.Length == 0 || values[0] == "")
            {
                continue;
            }

            var body_colum = new Dictionary<string, object>(); // 각 라인의 해당하는 값들을 읽는다.

            for (var j = 0; j < header_colum.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                object finalvalue = value;
                int n;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                body_colum[header_colum[j]] = finalvalue;
            }
            list.Add(body_colum);
        }
        return list;
    }   
}
