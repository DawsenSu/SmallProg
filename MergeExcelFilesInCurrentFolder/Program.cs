using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Packaging;

namespace MergeExcelFilesInCurrentFolder
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string _outputfilename = "MergedExecelWorkSheet";
                string _password = "L.L.Angela";
                var _excelfiles = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.xls");
                int _num;

                while (true)
                {
                    Console.Write("Please enter the maximum column number you want to merge(ie: A->1,B->2):");
                    string _numstr = Console.ReadLine();
                    if (!int.TryParse(_numstr, out _num) || _num < 1)
                    {
                        Console.WriteLine("Please enter valid number!");
                    }
                    else
                    {
                        break;
                    }
                }

                if (_excelfiles.Count() == 0)
                {
                    Console.WriteLine("There isn't excel in current folder, please check!");
                    Console.ReadKey();
                    return;
                }
                DataTable _dateTable = new DataTable(_outputfilename);

                bool _isFirstFile = true;
                foreach (var _excelfilePath in _excelfiles)
                {
                    using (ExcelPackage _package = new ExcelPackage(new FileInfo(_excelfilePath)))
                    {
                        ExcelWorksheet _sheet = _package.Workbook.Worksheets[1];
                        if (_isFirstFile)
                        {
                            for (int j = 1; j < _num + 1; j++)
                            {
                                if (_sheet.Cells[1, j].Value == null)
                                    _dateTable.Columns.Add("Column" + j);
                                else
                                    _dateTable.Columns.Add(_sheet.Cells[1, j].Value.ToString() + "Column" + j);
                            }
                            _isFirstFile = false;
                        }
                        for (int i = 2; i < _sheet.Dimension.Rows + 1; i++)
                        {
                            DataRow _newRow = _dateTable.NewRow();
                            bool _isAllEmpty = true;
                            for (int j = 1; j < _num + 1; j++)
                            {
                                if (_sheet.Cells[i, j].Value == null)
                                    _newRow[j - 1] = string.Empty;
                                else
                                {
                                    string _readstr = _sheet.Cells[i, j].Value.ToString();
                                    if (!string.IsNullOrEmpty(_readstr))
                                        _isAllEmpty = false;
                                    _newRow[j - 1] = _readstr;
                                }
                            }
                            if (_isAllEmpty) break;
                            _dateTable.Rows.Add(_newRow);
                        }
                    }
                    Console.WriteLine($"{_excelfilePath} read finished!");
                }

                Console.WriteLine($"Start to Output file!");
                if (_dateTable.Rows.Count > 1048500)
                {
                    StreamWriter _txtWt = File.CreateText(Directory.GetCurrentDirectory() + @"\" + _outputfilename + ".csv");
                    foreach (DataRow _row in _dateTable.Rows)
                    {
                        foreach (DataColumn _column in _dateTable.Columns)
                        {
                            _txtWt.Write(_row[_column]); _txtWt.Write(',');
                        }
                        _txtWt.WriteLine();
                    }
                    _txtWt.Close();
                    Console.WriteLine($"Merged File {_outputfilename}.csv has been saved to current folder!");
                }
                else
                {
                    using (ExcelPackage _mergedPackage = new ExcelPackage(new FileInfo(Directory.GetCurrentDirectory() + @"\" + _outputfilename + ".xlsx")))
                    {
                        ExcelWorksheet _sheet = _mergedPackage.Workbook.Worksheets.Add("MergedData");
                        _sheet.Cells["A1"].LoadFromDataTable(_dateTable, true, OfficeOpenXml.Table.TableStyles.Light1);
                        _mergedPackage.Save(_password);
                    }
                    Console.WriteLine($"Merged File {_outputfilename}.xlsx has been saved to current folder!");
                }
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();

            }

        }

    }
}
