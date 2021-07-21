using System;
using System.IO;
using Word = Microsoft.Office.Interop.Word;
using System.Web.Http;

namespace EmailService.Controllers
{

    class WordPDF
    {
        private object templateDirPath = "טקסט לתבנית אישור -עד 20 איש.docx";
        public WordPDF(string templatePath)
        {
            templateDirPath = templatePath;
        }

        public class Person
        {
            public string name;
            public string passportNumber;

            public bool IsValid()
            {
                return name != string.Empty && passportNumber != string.Empty;
            }
        }

        private void CreateNewFile(Word.Application application, Person[] peoples, string fileOutDirectory = "", string filenameTemplate = "confirmation", int page = 0)
        {
            //Test if template exists
            if (File.Exists((string)templateDirPath))
            {
                //Get the full path of the document
                object p = Path.GetFullPath((string)templateDirPath);
                //Getting instance of the document !Readonly is only in case if it was already open!
                Word.Document document = application.Documents.Open(ref p, ReadOnly: true);
                //Set the document is active on the main world instance
                document.Activate();
                //Get the first and the only table on the document
                Word.Table table = document.Tables[1];

                // Replace End date
                Word.Find findObject = application.Selection.Find;
                findObject.ClearFormatting();
                findObject.Text = "{T_DATE}";
                findObject.Replacement.ClearFormatting();
                findObject.Replacement.Text = $"{DateTime.Now.ToShortDateString()}";
                findObject.Execute(Replace: Word.WdReplace.wdReplaceAll);


                foreach (Word.Section item in document.Sections)
                {
                    Word.Range r = item.Headers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                    r.Find.Execute(FindText: "{DATE}", ReplaceWith: $"{DateTime.Now.ToShortDateString()}", Replace: Word.WdReplace.wdReplaceAll);
                }


                //Iterate over each person and adding its info to the table
                for (int i = 0; i < peoples.Length; i++)
                {
                    /*
                    Table start from one and not like a array from zero,
                    The first row is for titles so we need to offset our self,
                    from the second row so we need to offset by two
                    */

                    // Check in the person is valid no empty strings
                    if (peoples[i].IsValid())
                    {
                        //Add a row where the info would be entered
                        table.Rows.Add();
                        //Enter data to row
                        table.Cell(i + 2, 1).Range.Text = peoples[i].name;
                        table.Cell(i + 2, 2).Range.Text = peoples[i].passportNumber;
                    }
                }
                //Generate the new file name
                object filename = $"{filenameTemplate} {page + 1}.pdf";
                //Get the path for saving the file
                p = Path.Combine(fileOutDirectory, (string)filename);
                p = Path.GetFullPath((string)p);

                //Save as PDF savedas2 is from the MSDN website
                document.SaveAs2(ref p, Word.WdSaveFormat.wdFormatPDF);
                //Close the active documents and not saving any changes
                document.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
            }
            else
            {
                throw new FileNotFoundException("Template not found");
            }
        }

        /// <summary>
        /// This function He creates the documents needed by the
        /// </summary>
        /// <param name="peoples">Array of persons that will be filled in the PDF</param>
        /// <param name="fileOutDirectory">The directory where the files will be put it</param>
        /// <param name="fileNameTemplate">Filing template of the output files</param>
        public void CreateFiles(Person[] peoples, string fileOutDirectory = "", string fileNameTemplate = "confirmation")
        {
            /*
                Split the given list to 20 a time
            */

            //Launch a new Winword instance
            var application = new Word.Application();
            //Hide Word GUI
            application.Visible = true;

            for (int i = 0; i < Math.Ceiling(peoples.Length / 20.0); i++)
            {
                // save temporary array of peoples which is the size is between 0 to 20
                Person[] peopleT = new Person[Math.Min(20, peoples.Length - (i * 20))];
                //Copy sublist from main peoples to peopleT
                for (int j = 0; j < peopleT.Length; j++)
                {
                    peopleT[j] = peoples[j + i * 20];
                }
                // Create new PDF file from template with peopleT the info

                try
                {
                    this.CreateNewFile(application, peopleT, fileOutDirectory, fileNameTemplate, i);
                }
                catch (Exception)
                {

                    throw;
                }
                peopleT = null;
            }

            //Closing the application instance because we don't need it anymore
            application.Quit(SaveChanges: false);
        }
    }



    public class PDFController : ApiController
    {
        
        public async System.Threading.Tasks.Task<string> PostAsync()
        {
            return $"{Path.GetFullPath("doc")}";
        }


    }
}
