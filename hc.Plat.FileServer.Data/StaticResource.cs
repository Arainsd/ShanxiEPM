//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace hc.Plat.FileServer.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class StaticResource
    {
        public int Id { get; set; }
        public string App { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }
        public string Extension { get; set; }
        public long FileSize { get; set; }
        public string VirtualURL { get; set; }
        public string HttpURL { get; set; }
        public System.DateTime UploadTime { get; set; }
    }
}
