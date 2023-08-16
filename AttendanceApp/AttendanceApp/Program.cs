namespace AttendanceApp
{
    using System;
    using System.IO.Abstractions;
    using CommandLine;
    using IronOcr;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;    

    public static class Program
    {
        public class Options
        {
            [Option('g', "gather", Required = false, HelpText ="Data Gather Mode")]
            public string? Gather { get; set; }

            [Option('e', "export", Required = false, HelpText = "Export Data for MMYYYY")]
            public string? Export { get; set; }
        }


        public static void Main(string[] args)
        {
            Console.WriteLine("Program Start");
            
            try 
            {
                var builder = new ConfigurationBuilder();
                builder.AddJsonFile("appsettings.json");             
                var config = builder.Build();

                //var ocr = new IronTesseract();

                //using (var ocrInput = new OcrInput())
                //{
                //    ocrInput.AddImage("F:\\Black Desert\\nodewar\\attendance\\Storm_08162023.PNG");

                //    // Optionally Apply Filters if needed:
                //    // ocrInput.Deskew();  // use only if image not straight
                //    // ocrInput.DeNoise(); // use only if image contains digital noise
                //    ocrInput.Binarize();


                //    var ocrResult = ocr.Read(ocrInput);
                //    Console.WriteLine(ocrResult.Text);
                //}

                var mainWorkflow = new MainWorkflow(config);

                Parser.Default.ParseArguments<Options>(args)
                    .WithParsed<Options>(o =>
                    {
                        if (o.Gather != null)
                        {
                            Console.WriteLine("Gathering from: " + o.Gather);
                            //mainWorkflow.Gather("C:\\Users\\ReoSoul\\source\\repos\\AttendanceApp\\AttendanceApp\\Inputs");
                            mainWorkflow.Gather(o.Gather);
                        }
                        if (o.Export != null)
                        {
                            Console.WriteLine("Exporting: " + o.Export);
                            mainWorkflow.Export(o.Export);
                        }
                    });
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Unexpected Exception: {ex}");
            }
        }
    }
}
