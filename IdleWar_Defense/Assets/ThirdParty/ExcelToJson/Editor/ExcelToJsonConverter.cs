using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class ExcelToJsonConverter /*: IPreprocessBuildWithReport*/
{
    public int callbackOrder
    {
        get { return 0; }
    }

    // public void OnPreprocessBuild(BuildReport report)
    // {
    //     Debug.Log("Pre build for target " + report.summary.platform + " at path " + report.summary.outputPath);
    //     Convert();
    // }

    private static Dictionary<string, int> IDs;

    [MenuItem("DevTools/Excel To Json", priority = 21)]
    private static void Convert()
    {
        WriteIDs();
        WriteConstants();
        WriteLocalization();

        WriteDatas();

        //F5 Assets
        AssetDatabase.Refresh();
        Debug.Log("Convert Data Done");
    }

    private static void WriteIDs()
    {
        //IDs
        IDs = new Dictionary<string, int>();

        string txtPath = Application.dataPath + "/Core/Scripts/Config/IDs.cs";
        StreamWriter sw = new StreamWriter(txtPath, false);
        sw.Write("using System;\r\n");
        sw.Write("public class IDs\r\n{\r\n");

        string[] filePaths = Directory.GetFiles(Application.dataPath + "/Core/Excels/", "*.xlsx",
            SearchOption.TopDirectoryOnly);
        foreach (var path in filePaths)
        {
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
            XSSFWorkbook workbook = new XSSFWorkbook(file);

            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                string sheet_name = workbook.GetSheetName(i);
                if (sheet_name.Equals("IDs"))
                {
                    ISheet sheet = workbook.GetSheetAt(i);

                    IRow titleRow = sheet.GetRow(0);
                    int firstCellNum = titleRow.FirstCellNum;
                    int lastCellNum = titleRow.LastCellNum;
                    int cellCount = lastCellNum - firstCellNum;
                    while (cellCount % 3 != 0) cellCount++;
                    int lenght = cellCount / 3;

                    int lastRowNum = sheet.LastRowNum;
                    for (int j = 0; j < lenght; j++)
                    {
                        string title = titleRow.GetCell(firstCellNum + j * 3).ToString().Trim();
                        sw.Write("\t#region " + title + "\r\n");

                        for (int k = 1; k <= sheet.LastRowNum; k++)
                        {
                            IRow row = sheet.GetRow(k);

                            var cell = row.GetCell(firstCellNum + j * 3);
                            if (cell == null)
                            {
                                break;
                            }
                            else
                            {
                                var variable = cell.ToString().Trim();
                                if (variable.Equals(""))
                                {
                                    break;
                                }
                                else
                                {
                                    string value = row.GetCell(firstCellNum + j * 3 + 1).ToString().Trim();
                                    sw.Write("\tpublic const int " + variable + " = " + value + ";\r\n");
                                    IDs.Add(variable, int.Parse(value));
                                }
                            }
                        }

                        sw.Write("\t#endregion" + "\r\n \n \n");
                    }
                }
            }
        }

        sw.Write("}\r\n");
        sw.Close();
    }

    private static void WriteConstants()
    {
        Debug.Log("Skip write Constants");
        return;
        string txtPath = Application.dataPath + "/Core/Scripts/Config/Constants.cs";
        StreamWriter sw = new StreamWriter(txtPath, false);
        sw.Write("using System;\r\n");
        sw.Write("public class Constants\r\n{\r\n");

        string[] filePaths = Directory.GetFiles(Application.dataPath + "/Core/Excels/", "*.xlsx",
            SearchOption.TopDirectoryOnly);
        foreach (var path in filePaths)
        {
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
            XSSFWorkbook workbook = new XSSFWorkbook(file);
            Debug.LogError("get " + file.Name);
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {

                string sheet_name = workbook.GetSheetName(i);
                if (sheet_name.Equals("Constants"))
                {
                    Debug.LogError("get===> " + file.Name);
                    ISheet sheet = workbook.GetSheetAt(i);

                    IRow titleRow = sheet.GetRow(0);
                    int firstCellNum = titleRow.FirstCellNum;

                    int lastRowNum = sheet.LastRowNum;
                    for (int j = 1; j <= lastRowNum; j++)
                    {
                        IRow row = sheet.GetRow(j);

                        //Example
                        //CONSTANT_1  int       12
                        //CONSTANT_2  float     0.1
                        //CONSTANT_3  string    3
                        //CONSTANT_4  int[]     4
                        //CONSTANT_6  int[]     0:3:4:5
                        //CONSTANT_7  float[]   5:1:1:3
                        if (row != null)
                        {
                            var cell = row.GetCell(firstCellNum);
                            if (cell != null)
                            {
                                var variable = cell.ToString().Trim();
                                var type = row.GetCell(firstCellNum + 1).ToString().Trim();
                                var value = row.GetCell(firstCellNum + 2).ToString().Trim();
                                if (type.Contains("[]"))
                                {
                                    var values = value.Split('|'); //Constant đơn giản nên không làm kiểu " | " xuống dòng
                                    var newType = type.Replace("[]", "[" + values.Length + "]");

                                    value = "";
                                    string item;
                                    for (int k = 0; k < values.Length - 1; k++)
                                    {
                                        item = values[k];
                                        if (IDs.ContainsKey(item)) value += "IDs." + item + ",";
                                        else if (type.Equals("float[]")) value += item + "f,";
                                        else if (type.Equals("string[]")) value += "\"" + item + "\",";
                                        else value += item + ",";
                                    }

                                    item = values[values.Length - 1];
                                    if (IDs.ContainsKey(item)) value += "IDs." + item;
                                    else if (type.Equals("float[]")) value += item + "f";
                                    else if (type.Equals("string[]")) value += "\"" + item + "\"";
                                    else value += item + ",";

                                    sw.Write("\tpublic static readonly " + type + " " + variable + " = new " + newType +
                                             " {" + value + "};\r\n");
                                }
                                else
                                {
                                    if (IDs.ContainsKey(value)) value = "IDs." + value;
                                    else if (type.Equals("float")) value += "f";
                                    else if (type.Equals("string")) value = "\"" + value + "\"";
                                    sw.Write("\tpublic const " + type + " " + variable + " = " + value + ";\r\n");
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
        }

        sw.Write("}\r\n");
        sw.Close();
    }

    private static void WriteLocalization()
    {
        string txtPath = Application.dataPath + "/Core/Scripts/Config/Localization.cs";
        StreamWriter sw = new StreamWriter(txtPath, false);
        // sw.Write("using System;\r\n");
        // sw.Write("public class Constants\r\n{\r\n");

        string localizationTemplate = ReadString(Application.dataPath + "/Core/Excels/LocalizationTemplate.txt");
        string ID_HERE = "";
        string CONST_HERE = "";
        string STRING_HERE = "";
        string LANGUAGE_HERE = ""; //{ "english", "Localization_english" }, { "vietnamease", "Localization_vietnamease" },

        bool doneLanguageHere = false;
        int countKey = 0;
        List<string> languages = new List<string>();
        List<string> dataLanguages = new List<string>();

        string[] filePaths = Directory.GetFiles(Application.dataPath + "/Core/Excels/", "*.xlsx", SearchOption.TopDirectoryOnly);
        foreach (var path in filePaths)
        {
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
            XSSFWorkbook workbook = new XSSFWorkbook(file);

            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                string sheet_name = workbook.GetSheetName(i);
                if (sheet_name.Contains("Localization"))
                {
                    ISheet sheet = workbook.GetSheetAt(i);

                    IRow titleRow = sheet.GetRow(0);
                    int firstCellNum = titleRow.FirstCellNum;
                    int lastCellNum = titleRow.LastCellNum;
                    int lastRowNum = sheet.LastRowNum;
                    if (!doneLanguageHere)
                    {
                        //đoạn này lấy tên các loại ngôn ngử trong file excel
                        //{ "english", "Localization_english" }, { "vietnamease", "Localization_vietnamease" },
                        IRow row = sheet.GetRow(0);
                        for (int j = firstCellNum + 2; j < lastCellNum; j++)
                        {
                            var language = row.GetCell(j).ToString().Trim();
                            if (language == "desc") continue;
                            languages.Add(language);
                            dataLanguages.Add("");
                            LANGUAGE_HERE += "{ \"" + language + "\", \"" + "Localization_" + language + "\" }, ";
                        }

                        doneLanguageHere = true;
                    }

                    //đoạn này lấy ra id của mỗi từ trong file excel
                    //key code in Localization.cs
                    for (int j = 1; j <= lastRowNum; j++)
                    {
                        IRow row = sheet.GetRow(j);

                        //idString    relatedId    english    vietnamease    korean
                        if (row != null)
                        {
                            var cell = row.GetCell(firstCellNum);
                            if (cell != null && cell.CellType != CellType.Blank)
                            {
                                var idString = cell.ToString().Trim();
                                var cellNext = row.GetCell(firstCellNum + 1);
                                if (cellNext != null && cellNext.CellType != CellType.Blank)
                                {
                                    var relatedId = cellNext.ToString().Trim();
                                    if (IDs.ContainsKey(relatedId)) relatedId = IDs[relatedId] + "";
                                    idString += "_" + relatedId;
                                }

                                //HP = 0, MELEE_ATK,
                                if (countKey == 0) ID_HERE += idString + ", ";
                                else ID_HERE += idString + " = " + countKey + ", ";

                                //HP = 0, MELEE_ATK = 1,
                                CONST_HERE += idString + " = " + countKey + ", ";

                                //"HP", "MELEE_ATK",
                                STRING_HERE += "\"" + idString + "\", ";

                                countKey++;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    //đoạn này lấy data và lưu ra file
                    //["HP","Protect"]
                    //Localization_english
                    int indexLanguage = 0;// add tiếp vào các dataLanguage
                    for (int z = firstCellNum + 3; z < lastCellNum; z++)
                    {
                        for (int j = 1; j <= lastRowNum; j++)
                        {
                            IRow row = sheet.GetRow(j);

                            //idString    relatedId    english    vietnamease    korean
                            if (row != null)
                            {
                                var cell = row.GetCell(z);
                                if (cell != null)
                                {
                                    var value = cell.ToString().Trim();
                                    dataLanguages[indexLanguage] += "\"" + value + "\", ";
                                }
                                else
                                {
                                    continue;
                                }
                            }
                            else
                            {
                                continue;
                            }
                        }

                        indexLanguage++;
                    }
                    //
                }
            }
        }

        var count = dataLanguages.Count;
        for (var i = 0; i < count; i++)
        {
            var data = dataLanguages[i];

            //Localization_english
            string txtPathLanguage = Application.dataPath + "/Core/Resources/Data/Localization_" + languages[i] + ".txt";
            StreamWriter swLanguage = new StreamWriter(txtPathLanguage, false);
            swLanguage.Write("[" + data.Substring(0, data.Length - 2) + "]");
            swLanguage.Close();
        }

        localizationTemplate = localizationTemplate.Replace("{ID_HERE}", ID_HERE);
        CONST_HERE = CONST_HERE.Substring(0, CONST_HERE.Length - 2);
        localizationTemplate = localizationTemplate.Replace("{CONST_HERE}", CONST_HERE);
        localizationTemplate = localizationTemplate.Replace("{STRING_HERE}", STRING_HERE);
        localizationTemplate = localizationTemplate.Replace("{LANGUAGE_HERE}", LANGUAGE_HERE);

        sw.Write(localizationTemplate);
        sw.Close();
    }

    private static void WriteDatas()
    {
        //dòng thứ mấy chứa title
        var server_title = 0;
        //data bắt đầu từ dòng
        var server_num = 1;

        string[] filePaths = Directory.GetFiles(Application.dataPath + "/Core/Excels/", "*.xlsx",
            SearchOption.TopDirectoryOnly);
        foreach (var path in filePaths)
        {
            FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read);
            XSSFWorkbook workbook = new XSSFWorkbook(file);

            for (int z = 0; z < workbook.NumberOfSheets; z++)
            {
                string sheet_name = workbook.GetSheetName(z);
                if (!sheet_name.Equals("Constants") && !sheet_name.Equals("IDs") && !sheet_name.Contains("Localization"))
                {
                    ISheet sheet = workbook.GetSheetAt(z);

                    string txtPath = Application.dataPath + "/Core/Resources/Data/" + sheet_name + ".json";
                    StreamWriter sw = new StreamWriter(txtPath, false);
                    sw.Write("[\r\n");

                    IRow titleRow = sheet.GetRow(server_title);

                    int cellCount = titleRow.LastCellNum;
                    int rowCount = sheet.LastRowNum;

                    bool isID = false;
                    for (int i = server_num; i <= sheet.LastRowNum; i++)
                    {
                        IRow row = sheet.GetRow(i);

                        if (row == null)
                        {
                            Debug.Log("Error row！");
                            break;
                        }

                        string str_write = "\t{\r\n";
                        sw.Write(str_write);


                        string str_write2 = "";
                        //遍历该行的列
                        for (int j = row.FirstCellNum; j < cellCount; j++)
                        {

                            if (titleRow.GetCell(j) != null && titleRow.GetCell(j).ToString().Length != 0)
                            {
                                string value = "";
                                string mark = "";

                                if (row.GetCell(j) != null)
                                {
                                    //nếu cell là trống thì next, ko tạo thuộc tính json
                                    //nếu cell là string thì cho vào ""
                                    var cell = row.GetCell(j);
                                    if (cell.CellType == CellType.Blank) continue;
                                    else if (cell.CellType == CellType.String)
                                    {
                                        double checkNumber;
                                        var check = double.TryParse(cell.ToString().Trim(), out checkNumber);
                                        if (check) //nếu là number
                                        {
                                            mark = "";
                                        }
                                        else
                                        {
                                            mark = "\"";
                                        }
                                    }
                                    else mark = "";

                                    value = row.GetCell(j).ToString().Trim();
                                }

                                string title = titleRow.GetCell(j).ToString().Trim();
                                if (title.Contains("[]"))
                                {
                                    title = title.Replace("[]", "");
                                    value = value.Replace("|", ",");
                                    value = value.Replace(" ", "");//cho GD ko biết dấu space
                                    value = value.Replace("\n", "");//cho trường hợp xuống dòng
                                    value = value.Trim();
                                    var values = value.Split(',');
                                    var first = values[0];
                                    double checkNumber;
                                    var check = double.TryParse(first, out checkNumber);
                                    if (check) //nếu là number
                                    {
                                        str_write2 += "\t\t\"" + title + "\":[" + value + "],\r\n";
                                    }
                                    else //nếu là string hoặc boolean
                                    {
                                        isID = false;
                                        for (int k = 0; k < values.Length; k++)
                                        {
                                            var item = values[k];

                                            if (IDs.ContainsKey(item)) //nếu value là các IDs đã khai báo sẵn thì thay bằng giá trị IDs
                                            {
                                                isID = true;
                                                break;
                                            }
                                        }

                                        string newValues = "";
                                        string newValue;
                                        for (int k = 0; k < values.Length - 1; k++)
                                        {
                                            newValue = values[k];

                                            if (isID) //nếu value là các IDs đã khai báo sẵn thì thay bằng giá trị IDs
                                            {
                                                if (IDs.ContainsKey(newValue)) newValues += IDs[newValue] + ",";
                                                else newValues += newValue + ",";
                                            }
                                            else
                                            {
                                                if (newValue.Equals("TRUE") || newValue.Equals("FALSE"))
                                                    newValue = newValue.ToLower();
                                                else newValue = newValue.Replace("\n", "\\n");
                                                newValues += "\"" + newValue + "\",";
                                            }
                                        }

                                        newValue = values[values.Length - 1];

                                        if (isID)
                                        {
                                            if (IDs.ContainsKey(newValue)) newValues += IDs[newValue];
                                            else newValues += newValue;
                                        }
                                        else
                                        {
                                            if (newValue.Equals("TRUE") || newValue.Equals("FALSE"))
                                                newValue = newValue.ToLower();
                                            else newValue = newValue.Replace("\n", "\\n");
                                            newValues += "\"" + newValue + "\"";
                                        }

                                        str_write2 += "\t\t\"" + title + "\":[" + newValues + "],\r\n";
                                    }
                                }
                                else
                                {
                                    isID = false;
                                    if (IDs.ContainsKey(value))
                                    {
                                        str_write2 += "\t\t\"" + title + "\":" + IDs[value] + ",\r\n";
                                        isID = true;
                                    }

                                    if (!isID)
                                    {
                                        if (value.Equals("TRUE") || value.Equals("FALSE")) value = value.ToLower();
                                        else value = value.Replace("\n", "\\n");
                                        str_write2 += "\t\t\"" + title + "\":" + mark + value + mark + ",\r\n";
                                    }
                                }
                            }

                        }

                        int end_str2 = str_write2.LastIndexOf(",");
                        if (end_str2 != -1)
                        {
                            str_write2 = str_write2.Remove(end_str2, 1);
                            sw.Write(str_write2);
                        }

                        if (i == rowCount)
                        {
                            sw.Write("\t}\r\n");
                        }
                        else
                        {
                            IRow rownext = sheet.GetRow(i + 1);
                            if (row.GetCell(rownext.FirstCellNum) == null || rownext.GetCell(rownext.FirstCellNum).ToString().Length == 0)
                            {
                                sw.Write("\t}\r\n");
                                break;
                            }

                            sw.Write("\t},\r\n");
                        }
                    }

                    sw.Write("]\r\n");
                    sw.Close();
                }
            }
        }
    }

    private static string ReadString(string path)
    {
        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        var text = reader.ReadToEnd();
        reader.Close();

        return text;
    }
}
