using System;
using System.Drawing;
using System.Net.Mail;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using _Application = Microsoft.Office.Interop.Excel._Application;

using _Excel = Microsoft.Office.Interop.Excel;

namespace Kanban
{
    public partial class Kanban : Form
    {
        #region Initialize components

        private _Application _excelApp = new _Excel.Application();//Reference to app
        private Workbook _wb;//File opening
        private Worksheet _wsKanbanSheet;
        private Worksheet _wsMaterialIssued;

        //---------------------------------------------------------------------------------------------------------------------------------------
        private bool _closing = false;

        #endregion Initialize components

        #region Initialize Kanban

        public Kanban()
        {
            InitializeComponent();
        }

        #endregion Initialize Kanban

        #region Kanban_Load - open Kanban form then sheet 3 of Excel spreadsheet

        private void Kanban_Load(object sender, EventArgs e)
        {
            Show();
            string urlTest = @"C:\Users\GemmaThomas-Green\OneDrive - EFFECT Photonics\Visual Studio projects\Kanban project\Kanban Sheet NEW testing.xlsx";//In case I need it
            string urlKanbanSpreadSheet = "https://effectphotonicsltd.sharepoint.com/sites/Kanban/Shared%20Documents/Kanban%20Sheet%20NEW%20testing.xlsx?web=1";
            //Set 'Date' columnn to show current date
            ScanDataGrid.Columns["Date"].DefaultCellStyle.NullValue = DateTime.Now.ToString("dd/MM/yyyy");

            lblinfo.Text = "Excel file is opening";

            //Open Excel file from app
            _wb = _excelApp.Workbooks.Open(urlTest);

            ExcelFormatting();//Calls method - Puts back in the formatting

            lblinfo.Text = "Excel file is ready";

            //Sheet 3
            _wsMaterialIssued = _wb.Worksheets[3];
            _wsMaterialIssued.Activate();

            //Sheet 1
            _wsKanbanSheet = _wb.Worksheets[1];
        }

        #endregion Kanban_Load - open Kanban form then sheet 3 of Excel spreadsheet

        #region ScanDataGrid_CellClick - validate user input then decide to add them in to spreadsheet which sends an email if 'Current Stock' is lower than or equal to the 'Re-order Point'

        private void ScanDataGrid_CellClick(object sender, DataGridViewCellEventArgs cellEvent)
        {
            if (cellEvent.RowIndex >= 0)//Prevent 'CellClick' event to run when user clicks on header
            {
                DataGridViewRow row = ScanDataGrid.Rows[cellEvent.RowIndex];//Gets the current cell in the row

                if (cellEvent.ColumnIndex == 3)//'Insert into spreadsheet' column
                {
                    //Clear formatting so UsedRange works
                    _wsMaterialIssued.Rows.ClearFormats();

                    //Validate all data by the user
                    if (!ValidateUserInput(row))//If incorrect
                    {
                        return;//End function
                    }

                    //Go to sheet 1 to get information for the below 'if' and 'do/'while' statements
                    _wsKanbanSheet.Activate();
                    int excelRowIndex = 0;

                    lblinfo.Text = "Gathering from Excel, please wait";
                    Cursor.Current = Cursors.WaitCursor;

                    //Data validation----------------------------------------------------------------------------------------------------
                    //Make sure the 'Part Number' is one from the spreadsheet
                    if (!FindPartCodeRow(row.Cells["PartNumber"].Value.ToString(), ref excelRowIndex))
                    {
                        lblinfo.Text = "Excel file is ready";
                        Cursor.Current = Cursors.Default;

                        row.Cells["PartNumber"].Style.BackColor = Color.LightBlue;//Set colour of empty cell to blue

                        MessageBox.Show("Incorrect or no part number found", "Error/Missing", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        row.Cells["PartNumber"].Style.BackColor = Color.White;//Set colour of cell back to white
                        return;
                    }

                    lblinfo.Text = "Excel file is ready";
                    Cursor.Current = Cursors.Default;

                    //Data validation----------------------------------------------------------------------------------------------------
                    //Show the user what they're going to add into the spreadsheet is correct
                    if (MessageBox.Show("This is what will be added to the spreadsheet" +
                        "\n" + "\n" +
                        "Part Number\tDate\tQuantity Issued\n" +
                        row.Cells["PartNumber"].Value.ToString() + "\t" + DateTime.Now.ToString("dd/M/yyyy") + "\t" + row.Cells["QuantityIssued"].Value.ToString(),
                        "Are you sure this is what you want?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return;
                    }
                    //-------------------------------------------------------------------------------------------------------------------
                    lblinfo.Text = "Gathering from Excel, please wait";
                    Cursor.Current = Cursors.WaitCursor;

                    ExportToExcel(row);

                    string description = _wsKanbanSheet.Rows[excelRowIndex].Columns["A"].Value;
                    string KBNumber = _wsKanbanSheet.Rows[excelRowIndex].Columns["D"].Value;
                    double reOrderPoint = _wsKanbanSheet.Rows[excelRowIndex].Columns["T"].Value;
                    double currentStock = _wsKanbanSheet.Rows[excelRowIndex].Columns["W"].Value;

                    if (currentStock <= reOrderPoint)
                    {
                        SendEmail(description, KBNumber, reOrderPoint, currentStock);
                    }
                    else
                    {
                        TopMost = false;//MessageBox is on top
                        MessageBox.Show("No email sent");//Otherwise don't send message

                        TopMost = true;//Kanban app is on top

                        //If 'Current Stock' is more then don't send email and go back to sheet 3
                        _wsMaterialIssued.Activate();

                        lblinfo.Text = "Excel file is ready";
                        Cursor.Current = Cursors.Default;
                    }

                    lblinfo.Text = "Excel file is ready";
                    Cursor.Current = Cursors.Default;
                }
            }
            ExcelFormatting();//Calls method - Puts back in the formatting
        }

        #endregion ScanDataGrid_CellClick - validate user input then decide to add them in to spreadsheet which sends an email if 'Current Stock' is lower than or equal to the 'Re-order Point'

        #region ValidateUserInput - Checks the row of data by user

        private bool ValidateUserInput(DataGridViewRow row)
        {
            //Data validation----------------------------------------------------------------------------------------------------
            //Make sure the user has entered a value in the 'Part Number' and 'Quantity Issued' column
            if (row.Cells["PartNumber"].Value == null && row.Cells["QuantityIssued"].Value == null)
            {
                row.Cells["PartNumber"].Style.BackColor = Color.LightBlue;//Set colour of empty cell to blue
                row.Cells["QuantityIssued"].Style.BackColor = Color.LightBlue;//Set colour of empty cell to blue

                MessageBox.Show("These cells can't be empty", "Try again", MessageBoxButtons.OK, MessageBoxIcon.Error);

                row.Cells["PartNumber"].Style.BackColor = Color.White;//Set colour of cell back to white
                row.Cells["QuantityIssued"].Style.BackColor = Color.White;//Set colour of cell back to white

                return false;
            }

            //Data validation----------------------------------------------------------------------------------------------------
            //Make sure the user has entered a value in the 'Part Number' column
            if (row.Cells["PartNumber"].Value == null)
            {
                row.Cells["PartNumber"].Style.BackColor = Color.LightBlue;//Set colour of empty cell to blue

                MessageBox.Show("This cell can't be empty", "Try again", MessageBoxButtons.OK, MessageBoxIcon.Error);

                row.Cells["PartNumber"].Style.BackColor = Color.White;//Set colour of cell back to white

                return false;
            }

            //Data validation----------------------------------------------------------------------------------------------------
            //Make sure the user has entered a value in the 'Quantity Issued' column
            if (row.Cells["QuantityIssued"].Value == null)
            {
                row.Cells["QuantityIssued"].Style.BackColor = Color.LightBlue;//Set colour of empty cell to blue

                MessageBox.Show("This cell can't be empty", "Try again", MessageBoxButtons.OK, MessageBoxIcon.Error);

                row.Cells["QuantityIssued"].Style.BackColor = Color.White;//Set colour of cell back to white

                return false;
            }

            //Data validation----------------------------------------------------------------------------------------------------
            //Make sure the 'Quantity Issued' column is a number
            try
            {
                int quantity = int.Parse(row.Cells["QuantityIssued"].Value.ToString());
            }
            catch (FormatException)
            {
                row.Cells["QuantityIssued"].Style.BackColor = Color.LightBlue;//Set colour of empty cell to blue

                MessageBox.Show("You must enter a number for quantity", "Try again", MessageBoxButtons.OK, MessageBoxIcon.Error);

                row.Cells["QuantityIssued"].Style.BackColor = Color.White;//Set colour of cell back to white

                return false;
            }
            return true;
        }

        #endregion ValidateUserInput - Checks the row of data by user

        #region FindPartCodeRow - Find the Kanban Excel row the part code exists on, if not found return false

        private bool FindPartCodeRow(string partNumber, ref int excelRow)
        {
            Range range = _wsKanbanSheet.UsedRange;
            int lastRowKB = range.Rows.Count;//Get the last used row

            string[] KBNumber;//Array
            KBNumber = new string[lastRowKB];//Assign array to the lastRow -last used row

            //Loop through the lastRow (last empty used row)
            for (int firstRow = 2; firstRow <= lastRowKB; firstRow++)
            {
                //Each row of column A goes into the KBNumber array
                var test = _wsKanbanSheet.Rows[firstRow].Columns["D"].Value;
                if (test != null)
                {
                    KBNumber[firstRow - 2] = _wsKanbanSheet.Rows[firstRow].Columns["D"].Value.ToString();
                }
            }

            //Assign the int variable to the array of looking at the inputted part number is the same as the Excel 'Material Issued' tab column D
            int arrayIndex = Array.FindIndex(KBNumber, element => element == partNumber);

            int notFound = -1;
            if (arrayIndex == notFound)
            {
                return false;
            }
            else
            {
                excelRow = arrayIndex + 2;
                return true;
            }
        }

        #endregion FindPartCodeRow - Find the Kanban Excel row the part code exists on, if not found return false

        #region ExportToExcel - Insert ScanDataGrid row into spreadsheet

        private void ExportToExcel(DataGridViewRow row)
        {
            //Get the last used row and adds one
            Range userRange = _wsMaterialIssued.UsedRange;
            int lastRow = userRange.Rows.Count;
            int add = lastRow + 1;

            try
            {
                //'Part Number' column with last cell of Excel spreadsheet
                _wsMaterialIssued.Rows[add].Columns["A"] = row.Cells["PartNumber"].Value.ToString();

                //'Date' column with last cell of Excel spreadsheet
                _wsMaterialIssued.Rows[add].Columns["B"] = DateTime.Now.ToString("dd/M/yyyy");

                //'Quantity Issued' column with last cell of Excel spreadsheet
                _wsMaterialIssued.Rows[add].Columns["C"] = row.Cells["QuantityIssued"].Value.ToString();
            }
            catch
            {
                TopMost = false;//MessageBox is on top
                MessageBox.Show("You must enter a value in the cell", "Try again", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion ExportToExcel - Insert ScanDataGrid row into spreadsheet

        #region Email - Send email if 'Current Stock' is lower than or equal to the 'Re-order Point'

        private void SendEmail(string description, string KBNumber, double reOrderPoint, double currentStock)
        {
            //Open SMTP client for Office 365
            SmtpClient client = new SmtpClient();
            {
                client.SetDefaults();
            }

            //Define email message
            MailMessage message = new MailMessage();
            {
                //Set from address
                message.From = new MailAddress(client.GetSenderEmail());

                message.To.Add("Gemmathomasgreen@effectphotonics.com");

                //Set subject, body and the importance
                message.Subject = "New stock order on - " + DateTime.Now.ToString("dddd, dd MMMM yyyy") + " at " + DateTime.Now.ToString("HH:mm:ss");
                message.IsBodyHtml = true;
                string htmlString = "This item: " + "<b>" + description + "</b>" +
                                    " and KB Number: " + "<b>" + KBNumber + "</b>" +
                                    " needs to be restocked by " + "<u>at least</u>" + " this amount: " + "<b>" + reOrderPoint + "</b>" +
                                    ", where the current stock is: ";

                if (currentStock < 1)
                {
                    message.Body = htmlString + "<b style='color:red'>" + currentStock + "</b>";
                }
                else
                {
                    message.Body = htmlString + "<b>" + currentStock + "</b>";
                }

                message.Priority = MailPriority.High;

                //Send message
                client.Send(message);
                TopMost = false;//MessageBox is on top
                MessageBox.Show("Email sent");

                TopMost = true;//Kanban app is on top
            }
        }

        #endregion Email - Send email if 'Current Stock' is lower than or equal to the 'Re-order Point'

        //#region ScanDataGrid_KeyDown - After scanned barcode focus goes to next cell

        //private void ScanDataGrid_KeyDown(object sender, KeyEventArgs e)
        //{
        //    int column = ScanDataGrid.CurrentCell.ColumnIndex;
        //    int row = ScanDataGrid.CurrentCell.RowIndex;

        //    if (column < ScanDataGrid.Columns.Count - 1)
        //    {
        //        ScanDataGrid.CurrentCell = ScanDataGrid.Rows[row].Cells[column + 1];
        //        ScanDataGrid.Focus();
        //    }
        //    else if (column == ScanDataGrid.Columns.Count - 1)
        //    {
        //        ScanDataGrid.Rows.Add(1);
        //        ScanDataGrid.CurrentCell = ScanDataGrid.Rows[row].Cells[0];
        //        ScanDataGrid.Focus();
        //    }
        //}

        //#endregion ScanDataGrid_KeyDown - After scanned barcode focus goes to next cell

        #region Kanban_FormClosing - close Kanban form depending on if values are not null and when user exits straight after form opens

        private void Kanban_FormClosing(object sender, FormClosingEventArgs formClosing)
        {
            //Check if there's data in the rows and user says 'No'
            if (formClosing.CloseReason == CloseReason.UserClosing && ScanDataGrid.Rows.Count > 1 && ScanDataGrid.Rows != null && !_closing)
            {
                formClosing.Cancel = MessageBox.Show("Are you sure you want to exit?\nThere might be data that could be lost if you close?", "Close application", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No;

                lblinfo.Text = "Excel file is ready";
                Cursor.Current = Cursors.Default;
            }

            //When 'X' button is clicked only and/or user inputting a value and says 'Yes'
            _closing = !formClosing.Cancel;

            ExcelFormatting();//Calls method - Puts back in the formatting

            //If 'Yes' is selected, the Kanban form and and Excel spreadsheet closes
            if (_closing == true)
            {
                Cursor.Current = Cursors.WaitCursor;
                lblinfo.Text = "Excel file is closing, please wait";

                ExcelFormatting();//Calls method - Puts back in the formatting

                //Close Excel file
                _excelApp.Workbooks.Close();

                //Close Excel app
                _excelApp.Quit();
            }
            return;//If 'No' is selected, the Kanban form and Excel spreadsheet stays open
        }

        #endregion Kanban_FormClosing - close Kanban form depending on if values are not null and when user exits straight after form opens

        #region ExcelFormatting - Puts back in the formatting

        public void ExcelFormatting()
        {
            var wsMaterialIssued = _wb.Worksheets[3];

            //Set column B to date format
            wsMaterialIssued.Columns["B"].NumberFormat = "dd/MM/yyyy";

            //Add filter for column A
            wsMaterialIssued.Columns["A"].AutoFilter(1);

            //Change text size back to 12
            wsMaterialIssued.Columns["A:C"].Font.Size = 12;

            //Move text alignment back to the center
            wsMaterialIssued.Columns["A:C"].HorizontalALignment = XlHAlign.xlHAlignCenter;

            //Put back in the border
            wsMaterialIssued.Columns["A:C"].Borders.LineStyle = XlLineStyle.xlContinuous;

            //Set the first row to bold with gold background colour
            wsMaterialIssued.Rows["1"].Columns["A:C"].Font.Bold = true;

            wsMaterialIssued.Rows["1"].Columns["A:C"].Interior.Color = Color.FromArgb(255, 242, 204);
        }

        #endregion ExcelFormatting - Puts back in the formatting
    }
}