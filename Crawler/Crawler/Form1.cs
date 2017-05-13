using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using HtmlAgilityPack;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Globalization;

namespace Crawler
{
    public partial class Form1 : Form
    {

        public int parsed = 0;
        public List<listStruct> list_Data = new List<listStruct>();
        public int timer_count_times = 0;

        public int mean_timer_count = 0;
        public int page_num = 250;
        public int stop_page_num = 250;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("Sold");
            comboBox1.Items.Add("For Sale");
            comboBox1.Items.Add("For Lease");
            comboBox1.SelectedIndex = 0;
            button2.Enabled = false;
            //button3.Enabled = true;
            progressBar1.Style = ProgressBarStyle.Continuous;
            dateTimePicker1.Enabled = checkBox1.Checked;
            dateTimePicker2.Enabled = checkBox1.Checked;
        }

        public void setEnabled(bool enable)
        {
            comboBox1.Enabled = enable;
            textBox1.Enabled = enable;
            button1.Enabled = enable;
            button2.Enabled = enable;
            numericUpDown1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //date checking
            if (checkBox1.Checked == true)
            {
                DateTime first_date = dateTimePicker1.Value;
                DateTime second_date = dateTimePicker2.Value;
                if (first_date.ToLocalTime().Date > second_date.ToLocalTime().Date)
                {
                    MessageBox.Show("Check start date and end date!!!");
                    return;
                }
            }

            string searchKeywords = textBox1.Text;
            if (searchKeywords == "")
            {
                MessageBox.Show("Please enter Location.");
                return;
            }

            setEnabled(false);
            //button3.Enabled = true;
            list_Data.Clear();
            if (comboBox1.SelectedIndex == 0)
            {
                checkBox1.Enabled = false;
                dateTimePicker1.Enabled = false;
                dateTimePicker2.Enabled = false;
            }
            progressBar1.Maximum = 0;
            progressBar1.Value = 0;
            parsed = 0;
            label3.Text = "Site Connecting...";
            string url = "";
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    url = "http://www.realestate.com.au/sold";
                    break;
                case 1:
                    url = "http://www.realestate.com.au/buy";
                    break;
                case 2:
                    url = "http://www.realestate.com.au/rent";
                    break;
            }
            /*if (searchKeywords != "")
            {
                searchKeywords = searchKeywords.Replace(" ", "+");
                searchKeywords = searchKeywords.Replace(";", "%3b");
                url = url + "/in-" + searchKeywords;
            }*/
            timer1.Interval = 1000;
            timer2.Interval = 1000;
            timer_count_times = Convert.ToInt32(numericUpDown1.Value) * 60;
            mean_timer_count = Convert.ToInt32(numericUpDown1.Value) * 60;
            Thread callThread = new Thread(() => SearchTrigger(url, searchKeywords));
            callThread.IsBackground = true;
            callThread.Start();
        }

        public static string Substring(string Text, string FirstString = null, string LastString = null, StringComparison comparison = StringComparison.InvariantCulture)
        {
            string STR = Text;
            string STRFirst = FirstString;
            string STRLast = LastString;
            string FinalString;

            int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
            int Pos2 = STR.IndexOf(LastString);
            FinalString = STR.Substring(Pos1, Pos2 - Pos1);
            return FinalString;
        }

        public void SearchTrigger(string based_url, string searchKeywords)
        {
            //Building URLs
            List<string> urls = new List<string>();
            if (searchKeywords == "")
            {
                string url = based_url + "/list-0?source=location-search&activeSort=list-date";
                urls.Add(url);
            }
            else
            {
                string[] piece_keywords = searchKeywords.Split(new Char[] { ';' });
                foreach (string key in piece_keywords)
                {
                    string temps = key.Replace(" ", "+");
                    string url = based_url + "/in-" + temps + "/list-1?source=location-search&activeSort=list-date";
                    urls.Add(url);
                }
            }

            int pos = 0;
            int temp_mean_timer_count = mean_timer_count;

            stop_page_num = 20000;

            foreach (string url1 in urls)
            {
                int all_item_num = 0;

                pos++;
                mean_timer_count = temp_mean_timer_count;
                this.Invoke((MethodInvoker)delegate
                {
                    label7.Text = pos + " of " + urls.Count + " Searches";
                });

                string url = url1;
                for (int i = 1; i <= stop_page_num; i++)
                {
                    url = url.Replace("/list-" + (i - 1), "/list-" + i);

                    var web = new HtmlWeb();
                    var doc = web.Load(url);

                    ////////////////Pre-processing
                    HtmlNode node = doc.DocumentNode.SelectSingleNode("//div[contains(@id,'resultsInfo')]");
                    if (i == 1)
                    {
                        if (node != null)
                        {
                            HtmlNode node1 = node.ChildNodes["p"];
                            if (node1 != null)
                            {
                                string innerText = node1.InnerText;
                                innerText = Substring(innerText.ToLower(), "of", "total");
                                innerText = innerText.Replace(" ", "");
                                if (innerText != "")
                                {
                                    all_item_num = Convert.ToInt32(innerText);
                                    page_num = all_item_num / 20 + 1;
                                    if (stop_page_num == 20000)
                                        stop_page_num = page_num;
                                    //page_num = 2;
                                }
                                else
                                {
                                    all_item_num = page_num * 20;
                                }
                                this.Invoke((MethodInvoker)delegate
                                {
                                    progressBar1.Maximum = page_num;
                                });
                            }
                            else
                            {
                                this.Invoke((MethodInvoker)delegate
                                {
                                    label3.Text = "No Results";
                                });
                                break;
                            }
                        }
                        else
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                label3.Text = "No Results";
                            });
                            break;
                        }
                    }

                    if (node != null)
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            progressBar1.Value = i;
                        });
                    }
                    else
                    {
                        this.Invoke((MethodInvoker)delegate
                        {
                            progressBar1.Value = progressBar1.Maximum;
                        });
                        break;
                    }

                    this.Invoke((MethodInvoker)delegate
                    {
                        label3.Text = "Page " + i + " of " + page_num + " pages is being fetched...";
                    });

                    /////////////////////////////////
                    //Real Scraping starting...
                    HtmlNodeCollection nodeCollection = doc.DocumentNode.SelectNodes("//article[contains(@class,'resultBody ')]");
                    foreach (HtmlNode nodes in nodeCollection)
                    {
                        listStruct list_item = new listStruct();
                        var temp = new HtmlAgilityPack.HtmlDocument();
                        temp.LoadHtml(nodes.InnerHtml);
                        /*HtmlNode header_node = temp.DocumentNode.SelectSingleNode("//a[contains(@rel,'listingName')]");
                        string header = "";
                        if (header_node != null)
                            header = header_node.InnerText;*/

                        HtmlNode agent_node = temp.DocumentNode.SelectSingleNode("//figure[contains(@class,'listerName')]");
                        string agent = "";
                        if (agent_node != null)
                        {
                            agent = agent_node.InnerText;
                            agent = agent.Replace("Agent:", "");
                        }

                        HtmlNode date_node = temp.DocumentNode.SelectSingleNode("//p[contains(@class,'soldDate')]");
                        string date = "";
                        if (date_node != null)
                        {
                            date = date_node.InnerText;
                            date = date.Replace("Sold ", "");
                        }

                        //Agency//
                        HtmlNode detail_node = temp.DocumentNode.SelectSingleNode("//a[contains(@class,'detailsButton')]");
                        string agency = "";

                        string streetAddress = "";
                        string addressLocality = "";
                        string addressRegion = "";
                        string postalCode = "";
                        if (detail_node != null)
                        {
                            string detail_url = "http://www.realestate.com.au" + detail_node.Attributes["href"].Value;
                            var web1 = new HtmlWeb();
                            var doc1 = web.Load(detail_url);

                            HtmlNode agency_node = doc1.DocumentNode.SelectSingleNode("//p[contains(@class,'agencyName')]");
                            if (agency_node != null)
                            {
                                agency = agency_node.InnerText;
                                agency = agency.Replace("&amp;", "&");
                                agency = agency.Replace("&#039;", "'");
                                string[] pie = agency.Split(new String[] { " - " }, StringSplitOptions.None);
                                if (pie.Count() > 1)
                                    agency = pie[0];
                            }

                            HtmlNode streetAddress_node = doc1.DocumentNode.SelectSingleNode("//span[contains(@itemprop,'streetAddress')]");
                            streetAddress = "";
                            if (streetAddress_node != null)
                                streetAddress = streetAddress_node.InnerText;

                            HtmlNode addressLocality_node = doc1.DocumentNode.SelectSingleNode("//span[contains(@itemprop,'addressLocality')]");
                            addressLocality = "";
                            if (addressLocality_node != null)
                                addressLocality = addressLocality_node.InnerText;

                            HtmlNode addressRegion_node = doc1.DocumentNode.SelectSingleNode("//span[contains(@itemprop,'addressRegion')]");
                            addressRegion = "";
                            if (addressRegion_node != null)
                                addressRegion = addressRegion_node.InnerText;

                            HtmlNode postalCode_node = doc1.DocumentNode.SelectSingleNode("//span[contains(@itemprop,'postalCode')]");
                            postalCode = "";
                            if (postalCode_node != null)
                                postalCode = postalCode_node.InnerText;
                        }

                        list_item.year = date;
                        list_item.streetAddress = streetAddress;
                        list_item.addressLocality = addressLocality;
                        list_item.addressRegion = addressRegion;
                        list_item.postalCode = postalCode;
                        list_item.agent = agent;
                        list_item.agency = agency;

                        list_Data.Add(list_item);

                        parsed++;
                        this.Invoke((MethodInvoker)delegate
                        {
                            label3.Text = "Page " + i + " of " + page_num + " pages is being fetched...\r\n\r\n" + parsed + " of " + all_item_num + " items were fetched!";
                        });
                    }
                    stop_page_num = Convert.ToInt32(numericUpDown2.Value);
                }

                if (pos < urls.Count)
                {
                    //TIMER Function
                    this.Invoke((MethodInvoker)delegate
                    {
                        timer2.Enabled = true;
                        timer2.Start();
                    });
                    while (mean_timer_count > 0) ;
                    this.Invoke((MethodInvoker)delegate
                    {
                        timer2.Enabled = false;
                        timer2.Stop();
                    });
                }
            }

            if (list_Data.Count > 0)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    //button3.Enabled = false;
                    label3.Text = "Finished!!! \r\n\r\n You can export fetched data into csv/excel format by clicking \"Export\" button.";
                    button2.Enabled = true;
                });
            }

            this.Invoke((MethodInvoker)delegate
            {
                timer1.Enabled = true;
                timer1.Start();
            });
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        public void ExportIntoCsv(string fileName)
        {
            /*//Exporting
            Excel.Application xlApp;
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;

            object misValue = System.Reflection.Missing.Value;
            try
            {
                xlApp = new Excel.Application();

                xlWorkBook = xlApp.Workbooks.Add(misValue);
                xlApp.DisplayAlerts = false;
                //if (System.IO.File.Exists(fileName) == true)
                //    xlApp.DisplayAlerts = true;

                xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

                for (int i = 0; i <= list_Data.Count; i++)
                {
                    if (i == 3000 || i == 2000 || i == 1000 || i == 500 || i == 4000 || i == 1200 || i == 1400 || i == 1600 || i == 1800 || i == 2200 || i == 2400 || i == 2600 || i == 2800)
                        i = i;
                    if (i > 0)
                    {
                        string full_addr = list_Data[i-1].address;
                        string[] pieces = full_addr.Split(new Char[] {','});
                        xlWorkSheet.Cells[i + 1, 1] = pieces[0];
                        xlWorkSheet.Cells[i + 1, 2] = pieces[1];
                        xlWorkSheet.Cells[i + 1, 3] = pieces[2];
                        xlWorkSheet.Cells[i + 1, 4] = list_Data[i-1].agent;
                    }
                    else
                    {
                        xlWorkSheet.Cells[1, 1] = "Address";
                        xlWorkSheet.Cells[1, 2] = "State";
                        xlWorkSheet.Cells[1, 3] = "Postcode";
                        xlWorkSheet.Cells[1, 4] = "Agent";
                        xlWorkSheet.Cells[1, 5] = "Date";
                    }
                }
                xlWorkBook.SaveAs(fileName, Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                xlWorkBook.Close(true, misValue, misValue);
                MessageBox.Show("Excel file created , you can find the file " + fileName);

                xlApp.Quit();
            }
            catch
            {
                this.Invoke(new System.Action(() =>
                {
                    MessageBox.Show("Please Install MS Excel!");
                }));
            }*/

            File.WriteAllText(fileName, "Address,Suburb,State,PostCode,Agent,Agency,Date\r\n");
            bool checking = true;
            for (int i = 1; i <= list_Data.Count; i++)
            {
                checking = true;
                string date = list_Data[i-1].year;
                if (checkBox1.Checked == true && date != "")
                {
                    DateTime first_date = dateTimePicker1.Value.ToLocalTime().Date;
                    DateTime second_date = dateTimePicker2.Value.ToLocalTime().Date;

                    date = date.Replace("-", " ");
                    string[] pieces = date.Split(new Char[] { ' ' });
                    int day = Convert.ToInt32(pieces[1]);
                    string mon = pieces[2];
                    int year = 2000 + Convert.ToInt32(pieces[3]);
                     
                    int month = DateTime.ParseExact(mon, "MMM", CultureInfo.InvariantCulture).Month;
                    DateTime dt = new DateTime(year, month, day);

                    if (!(dt >= first_date && dt <= second_date))
                        checking = false;
                }
                if (checking)
                {
                    File.AppendAllText(fileName, list_Data[i - 1].streetAddress + "," + list_Data[i - 1].addressLocality + "," + list_Data[i - 1].addressRegion + "," + list_Data[i - 1].postalCode + "," + list_Data[i - 1].agent + "," + list_Data[i - 1].agency + "," + list_Data[i - 1].year + "\r\n");
                }
            }
            MessageBox.Show("Excel file created , you can find the file " + fileName);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ///Exporting
            String fileName = "";
            saveFileDialog1.OverwritePrompt = true;
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result != DialogResult.OK) // Test result.
                return;
            fileName = saveFileDialog1.FileName;
            if (fileName != "")
            {
                if (fileName.Substring(fileName.Length - 4) != @".csv")
                {
                    fileName += ".csv";
                }
                ExportIntoCsv(fileName);
            }
            ///
            //button2.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (timer_count_times > 0)
            {
                label3.Text = "Finished!!! \r\n\r\n You can export fetched data into csv/excel format by clicking \"Export\" button.\r\n\r\n" + (timer_count_times / 60).ToString("00") + " : " + (timer_count_times % 60).ToString("00");
                timer_count_times--;
            }
            else
            {
                label3.Text = "You can do search now.";
                button1.Enabled = true;
                comboBox1.Enabled = true;
                textBox1.Enabled = true;
                timer1.Enabled = false;
                numericUpDown1.Enabled = true;
                if (comboBox1.SelectedIndex == 0)
                {
                    checkBox1.Enabled = true;
                    dateTimePicker1.Enabled = true;
                    dateTimePicker2.Enabled = true;
                }
            }
        }

        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            //Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            //buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            //buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 220, 13);
            textBox.SetBounds(12, 36, 200, 20);
            buttonOk.SetBounds(228, 34, 75, 23);
            //buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            //buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(320, 80);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk/*, buttonCancel*/ });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            //form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string value = stop_page_num.ToString();
            if (InputBox("Page Setting", "Page Number:", ref value) == DialogResult.OK)
            {
                stop_page_num = Convert.ToInt32(value);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 0)
            {
                checkBox1.Checked = false;
                checkBox1.Enabled = true;
            }
            else
            {
                checkBox1.Checked = false;
                checkBox1.Enabled = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePicker1.Enabled = checkBox1.Checked;
            dateTimePicker2.Enabled = checkBox1.Checked;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (mean_timer_count > 0)
            {
                label3.Text = "Waiting...\r\n\r\n" + (mean_timer_count / 60).ToString("00") + " : " + (mean_timer_count % 60).ToString("00");
                mean_timer_count--;
            }
            else
            {
                timer2.Stop();
                timer2.Enabled = false;
            }
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            stop_page_num = Convert.ToInt32(numericUpDown2.Value);
        }

        private void numericUpDown2_Enter(object sender, EventArgs e)
        {
            //stop_page_num = Convert.ToInt32(numericUpDown2.Value);
        }

        private void numericUpDown2_KeyDown(object sender, KeyEventArgs e)
        {
            stop_page_num = Convert.ToInt32(numericUpDown2.Value);
        }
    }

    public class listStruct
    {
        public string streetAddress;
        public string addressLocality;
        public string addressRegion;
        public string postalCode;
        public string agent;
        public string year;
        public string agency;

        public listStruct()
        {
            agency = "";
            streetAddress = "";
            addressLocality = "";
            addressRegion = "";
            postalCode = "";
            agent = "";
            year = "";
        }
    }
}
