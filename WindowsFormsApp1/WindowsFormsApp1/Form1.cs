using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;


namespace OreillyTriviaHarvester
{
    public partial class OreillyTriviaHarvester : Form
    {
        private int CurrentPage = 1;
        private bool isRightPage = true;
        public OreillyTriviaHarvester()
        {
            InitializeComponent();
        }

        private void ExportDataSetToExcel(DataSet DS, string SavePath)
        {
            //Creae an Excel application instance
            Excel.Application excelApp = new Excel.Application();

            var NewWB = excelApp.Workbooks.Add();
            NewWB.SaveAs(SavePath);
            NewWB.Close();

            //Create an Excel workbook instance and open it from the predefined location
            Excel.Workbook excelWorkBook = excelApp.Workbooks.Open(SavePath);

            foreach (DataTable table in DS.Tables)
            {
                //Add a new worksheet to workbook with the Datatable name
                Excel.Worksheet excelWorkSheet = excelWorkBook.Sheets.Add();
                excelWorkSheet.Name = table.TableName;

                for (int i = 1; i < table.Columns.Count + 1; i++)
                {
                    excelWorkSheet.Cells[1, i] = table.Columns[i - 1].ColumnName;
                }

                for (int j = 0; j < table.Rows.Count; j++)
                {
                    for (int k = 0; k < table.Columns.Count; k++)
                    {
                        excelWorkSheet.Cells[j + 2, k + 1] = table.Rows[j].ItemArray[k].ToString();
                    }
                }
            }

            excelWorkBook.Save();
            excelWorkBook.Close();
            excelApp.Quit();

            Marshal.ReleaseComObject(NewWB);
            Marshal.ReleaseComObject(excelWorkBook);
            Marshal.ReleaseComObject(excelApp);

            CurrentPage++;
            WebBrowser.Navigate("https://www.billoreilly.com/quiz?action=viewQuiz&categoryID=1&quizID=" + CurrentPage);
        }

        private void SecondPageAction()
        {
            //Play in the Answer Page.
            try
            {
                HtmlElementCollection answerpage_td_elements = WebBrowser.Document.GetElementsByTagName("td");
                List<Dictionary<string, string>> list_result = new List<Dictionary<string, string>>();
                foreach (HtmlElement item in answerpage_td_elements)
                {
                    Dictionary<string, string> result = null;

                    if (item.GetAttribute("id") == "bold" && item.GetAttribute("className") == "defaultBody")
                    {
                        result = new Dictionary<string, string>();
                        result["problem"] = item.InnerText.Substring(3);

                        int neg_index = 0;
                        HtmlElement AnswerRow = item.Parent.NextSibling;
                        //HtmlElement AnswerTable = AnswerRow.Document.GetElementsByTagName("table")[0];
                        HtmlElementCollection answer_table_rows = AnswerRow.GetElementsByTagName("tr");
                        foreach (HtmlElement index_tr in answer_table_rows)
                        {
                            if (index_tr.GetElementsByTagName("td").Count == 2)
                            {
                                StyleGenerator sg = new StyleGenerator();
                                sg.ParseStyleString(index_tr.Style);
                                string bk_color = sg.GetStyle(" BACKGROUND-COLOR");
                                if (bk_color == " #e9f5e4")
                                {
                                    result["positive"] = index_tr.GetElementsByTagName("td")[1].InnerText;
                                }
                                else if (bk_color == " #fff")
                                {
                                    string temp_index_string = "negative" + (++neg_index);
                                    result[temp_index_string] = index_tr.GetElementsByTagName("td")[1].InnerText;
                                }
                            }
                        }

                        list_result.Add(result);
                    }
                }

                //Write Into DataTable
                int MaxListCount = list_result.OrderByDescending(x => x.Count).First().Count;

                DataTable IndexTable = new DataTable("Q&A");
                IndexTable.Columns.Add("Question");
                IndexTable.Columns.Add("Positive");

                for (int i=0; i<MaxListCount-2; i++)
                    IndexTable.Columns.Add("Negative"+(i+1));

                foreach (Dictionary<string, string> item in list_result)
                {
                    DataRow TempRow = IndexTable.NewRow();
                    TempRow["Question"] = item["problem"];
                    TempRow["Positive"] = item["positive"];
                    for (int i = 0; i < MaxListCount-2; i++)
                    {
                        string trkey = "Negative" + (i+1);
                        string ikey = "negative" + (i+1);
                        if (i < item.Count-2)
                        {
                            TempRow[trkey] = item[ikey];
                        }
                        else
                            TempRow[trkey] = "";
                    }
                    IndexTable.Rows.Add(TempRow);
                }

                //Create a DataSet with the existing DataTables
                DataSet ds = new DataSet("Q & A");
                ds.Tables.Add(IndexTable);

                string path = SavePath.Text + @"\Result"+CurrentPage+".xlsx";
                ExportDataSetToExcel(ds, path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var s = WebBrowser.ReadyState;
            if (s == WebBrowserReadyState.Interactive)
            {
                isRightPage = false;
                HtmlElementCollection firstpage_img_elements = WebBrowser.Document.GetElementsByTagName("img");
                foreach (HtmlElement item in firstpage_img_elements)
                {
                    if (item.GetAttribute("src") == "https://www.billoreilly.com/images/icons/small/folderIcon.png")
                    {
                        isRightPage = true;
                        break;
                    }
                }
                if (isRightPage)
                {
                    PageNumPanel.Text = CurrentPage.ToString();
                    //Find the First Page's Submit Button
                    HtmlElementCollection firstpage_input_elements = WebBrowser.Document.GetElementsByTagName("input");
                    HtmlElement wanted = firstpage_input_elements[0];

                    int found = 0;
                    foreach (HtmlElement item in firstpage_input_elements)
                    {
                        if (item.GetAttribute("value") == "Submit")
                        {
                            wanted = item;
                            found = 1;
                            break;
                        }
                    }

                    //Do the form Submit in order to navigate to the answer page
                    if (found == 1)
                        wanted.InvokeMember("click");
                    else
                    {
                        SecondPageAction();
                    }
                }
                else
                {
                    MessageBox.Show("No more Q&A Pages");
                    Application.Exit();
                }
            }
        }


        private void OnStart(object sender, EventArgs e)
        {
            WebBrowser.ScriptErrorsSuppressed = true;
            CurrentPage = (int) BeginPage.Value;
            BtnStart.Enabled = false;

            WebBrowser.Navigate("https://www.billoreilly.com/quiz?action=viewQuiz&categoryID=1&quizID="+CurrentPage);
            //WebBrowser.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(webBrowser_DocumentCompleted);
            //CurrentPage++;

            //if (!isRightPage && CurrentPage == BeginPage.Value)
                //MessageBox.Show("This is not the Q&A Page!");
        }

        private void OnChooseSaveFolder(object sender, EventArgs e)
        {
            if (FolderBrowserDlg.ShowDialog() == DialogResult.OK)
            {
                SavePath.Text = FolderBrowserDlg.SelectedPath;
            }
        }
    }
}
