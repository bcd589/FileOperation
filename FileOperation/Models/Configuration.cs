using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FileOperation.Models
{
    /// <summary>
    /// 配置类，用于保存和加载用户配置
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// 历史配置ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 源文件夹路径
        /// </summary>
        public string SourcePath { get; set; }
        
        /// <summary>
        /// 目标文件夹路径
        /// </summary>
        public string TargetPath { get; set; }
        
        /// <summary>
        /// 扩展名列表（无点号，如["txt","md"]）
        /// </summary>
        [XmlArray("Extensions")]
        [XmlArrayItem("Extension")]
        public List<string> Extensions { get; set; }
        
        /// <summary>
        /// 配置创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        
        public Configuration()
        {
            Extensions = new List<string>();
            CreateTime = DateTime.Now;
        }
    }
}