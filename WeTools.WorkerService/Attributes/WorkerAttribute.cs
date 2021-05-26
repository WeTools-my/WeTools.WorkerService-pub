using System;

namespace WeTools.WorkerService.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WorkerAttribute : Attribute
    {
        public WorkerAttribute(string name) {
            Name = name;
        }

        /// <summary>
        /// Worker名称，此名称与配置文件中节点名称保持一致
        /// </summary>
        public string Name { get; }

    }
}
