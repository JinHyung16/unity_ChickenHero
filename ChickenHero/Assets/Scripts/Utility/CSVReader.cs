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

        var lines = Regex.Split(data.text, splite_read_line); // CSV������ �ۼ��Ǿ� �ִ� ��� ������ �о� ���μ��� �����Ѵ�.

        if (lines.Length <= 1)
        {
            return list;
        }

        var header_colum = Regex.Split(lines[0], splite_read); // �� �� ù ���� �д´�.

        for (var i = 1; i < lines.Length; i++) // header�� ������ ���� �ٺ��� ������ ���پ� �� �д´�.
        {
            var values = Regex.Split(lines[i], splite_read);
            if (values.Length == 0 || values[0] == "")
            {
                continue;
            }

            var body_colum = new Dictionary<string, object>(); // �� ������ �ش��ϴ� ������ �д´�.

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
