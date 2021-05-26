using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WeTools.WorkerService.Model;

namespace WeTools.WorkerService.Options
{
    public class WeToolWorkersOption 
    {
        //[Required(AllowEmptyStrings = false, ErrorMessage = "缺少Worker目录，请检查Service:Dir节点")]
        public string Dir { get; set; } = "/";

        [Required(ErrorMessage = "缺少Worker配置，请检查WeTools:Workers节点")]
        public List<WeToolWorkerConfig> Workers { get; set; }
    }
}
