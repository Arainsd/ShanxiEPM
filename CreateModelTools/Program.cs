using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using AutoMapper;


namespace CreateModelTools
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Mapper.Initialize(cfg =>
            {
                AutoMapper.Mappers.MapperRegistry.Mappers.Add(new AutoMapper.Data.DataReaderMapper { YieldReturnEnabled = true });
                cfg.CreateMap<System.Data.IDataReader, TableColumn>();
            });
            Application.Run(new MainForm());
        }
    }
}
