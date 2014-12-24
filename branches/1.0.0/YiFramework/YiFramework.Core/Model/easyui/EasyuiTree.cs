using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace YiFramework.Core
{
    public class EasyuiTree
    {
        public string id { get; set; }
        public string text { get; set; }
        public string iconCls { get; set; }
        public string state { get; set; }
        public bool @checked { get; set; }
        public object attributes { get; set; }
        
        [JsonIgnoreAttribute]
        public string pid { get; set; }
        [JsonIgnoreAttribute]
        public int sort { get; set; }

        public IEnumerable<EasyuiTree> children { get; set; }

        /// <summary>
        /// 把当前节点及子节点转化为一个集合
        /// </summary>
        /// <returns>没有父子关系的所有节点集合</returns>
        public List<EasyuiTree> ToList() 
        {
            List<EasyuiTree> result = new List<EasyuiTree>();
            result.Add(this);
            _getChildrenList(this.children, result);
            return result;
        }

        /// <summary>
        /// 把树形结构的集合转换为非树形结构的集合
        /// </summary>
        /// <param name="treeList"></param>
        /// <returns></returns>
        public static List<EasyuiTree> ToList(List<EasyuiTree> treeList)
        {
            List<EasyuiTree> result = new List<EasyuiTree>();
            foreach (var item in treeList)
            {
                result.Add(item);
                _getChildrenList(item.children, result);
            }
            return result;
        }

        /// <summary>
        /// 把树节点集合 根据id和pid关系创建成树结构
        /// <param name="list">所有节点集合</param>
        /// </summary>
        /// <returns>父子关系的一串树节点集合</returns>
        public static List<EasyuiTree> ToTreeList(List<EasyuiTree> list)
        {
            return ToTreeList(list, a => a.pid == null || string.IsNullOrEmpty(a.pid));
        }

        /// <summary>
        /// 把树节点集合 根据id和pid关系创建成树结构
        /// <param name="list">所有节点集合</param>
        /// <param name="rootLamb">父节点条件表达式</param>
        /// </summary>
        /// <returns>父子关系的一串树节点集合</returns>
        public static List<EasyuiTree> ToTreeList(List<EasyuiTree> list,Func<EasyuiTree,bool> rootLamb)
        {
            if (!list.Any()) { return null; }
            var result = list
                .Where(rootLamb)
                .OrderBy(a => a.sort)
                .Select(a => new EasyuiTree
                {
                    id = a.id,
                    text = a.text,
                    pid=a.pid,
                    iconCls = a.iconCls,
                    @checked = a.@checked,
                    attributes = a.attributes,
                    state = a.state,
                    children = _getTreeChildren(list, (b => b.pid == a.id))
                });
            return result.ToList();
        }

        /// <summary>
        /// 已知所有节点和父级节点，获取父节点下的所有子节点
        /// </summary>
        /// <param name="allList">所有子节点</param>
        /// <param name="rootId">作为根父节点的id</param>
        /// <returns>所有子节点（没有父级关系）集合</returns>
        public static List<EasyuiTree> GetChildren(List<EasyuiTree> allList, string rootId) 
        {
            List<EasyuiTree> childrens = new List<EasyuiTree>();
            _getChildrenList(allList, childrens, rootId);
            return childrens;
        }

        /// <summary>
        /// 获取所有父节点直到根节点
        /// </summary>
        /// <param name="allList"></param>
        /// <param name="childId"></param>
        /// <returns></returns>
        public static List<EasyuiTree> GetParents(List<EasyuiTree> allList, string childId) 
        {
            var child = allList.SingleOrDefault(a => a.id == childId);
            List<EasyuiTree> result = new List<EasyuiTree>();
            if (child != null)
            {
                getModuleParents(result, allList, child.pid);
            }
            return result;
        }

        //获取父节点
        private static void getModuleParents(List<EasyuiTree> ps, List<EasyuiTree> allModule, string pid)
        {
            var p = allModule.SingleOrDefault(a => a.id == pid);
            if (p != null)
            {
                ps.Add(p);
                if (p.pid != null)
                {
                    getModuleParents(ps, allModule, p.pid);
                }
            }
        }

        //获取子节点
        private static void _getChildrenList(List<EasyuiTree> allList, List<EasyuiTree> children, string id)
        {
            var son = allList.Where(a => a.pid == id);
            if (son.Any())
            {
                foreach (var item in son)
                {
                    children.Add(item);
                    _getChildrenList(allList, children, item.id);
                }
            }
        }

        //获取子节点列表
        private static void _getChildrenList(IEnumerable<EasyuiTree> children, List<EasyuiTree> result) 
        {
            foreach (var item in children)
            {
                result.Add(item);
                if (item.children != null) 
                {
                    _getChildrenList(item.children, result);
                }
            }
        }

        //获取树形子节点列表
        private static List<EasyuiTree> _getTreeChildren(IEnumerable<EasyuiTree> listAll, Func<EasyuiTree, bool> whereLamb)
        {
            if (!listAll.Any() || !listAll.Where(whereLamb).Any()) { return null; }
            var result = listAll
                .Where(whereLamb)
                .OrderBy(a => a.sort)
                .Select(a => new EasyuiTree
                {
                    id = a.id,
                    pid=a.pid,
                    text = a.text,
                    iconCls = a.iconCls,
                    @checked=a.@checked,
                    attributes=a.attributes,
                    state=a.state,
                    children = _getTreeChildren(listAll,(b=>a.id==b.pid))
                });
            return result.ToList();
        }
    }
}
