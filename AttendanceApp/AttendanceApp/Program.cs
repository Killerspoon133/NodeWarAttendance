namespace AttendanceApp
{
    using System;
    using CommandLine;
    using Microsoft.Extensions.Configuration;

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

                var mainWorkflow = new MainWorkflow(config);

                Parser.Default.ParseArguments<Options>(args)
                    .WithParsed<Options>(o =>
                    {
                        if (o.Gather != null)
                        {
                            Console.WriteLine("Gathering from: " + o.Gather);
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
