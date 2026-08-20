//试题分类的选择组件
Vue.component('selectparts', {
    props: ['orgid'],
    data: function () {
        return {
            search: '',  //检索字符串    
            datas: [],      //试题分类数据
            props: {
                children: 'children',
                label: 'Qp_Name'
            },
            selectarr: [],   //所选试题分类
            loading: false
        }
    },
    watch: {
        'orgid': {
            handler: function (nv, ov) {
                this.getdatas().then(function ([th,data]) {
                    th.$emit('load', data);
                });
            }, immediate: true, deep: true
        },
        //过滤树形数据
        search: function (val) {
            this.$refs.datatree.filter(val);
        },
    },
    computed: {},
    mounted: function () {

    },
    methods: {
        //所取试题分类的数据，为树形数据
        getdatas: function () {
            var th = this;
            return new Promise(function (resolve, reject) {
                th.loading = true;
                $api.get('ExamQues/PartTree', { orgid: th.orgid, search: '', isuse: true })
                    .then(function (req) {
                        if (req.data.success) {
                            th.datas = req.data.result;
                            resolve([th,th.datas]);
                        } else {
                            throw req.data.message;
                        }
                    }).catch(err => console.error(err))
                    .finally(() => th.loading = false);
            });
        },
        //过滤树形
        filterNode: function (value, data) {
            if (!value) return true;
            var txt = $api.trim(value.toLowerCase());
            if (txt == '') return true;
            return data.Qp_Name.toLowerCase().indexOf(txt) !== -1;
        },
        //选择变更
        handleCheckChange: function (data, checked, indeterminate) {
            //当前选中的数据
            //如果某个节点下的下级节点全部选中了，则只取当前节点；只有是半选，返回选中的子节点，且不包括自身（半选中）的节点。
            let nodes = this.getProcessedCheckedKeys();
            this.selectarr = nodes;
            this.$emit('select', nodes);
        },
        // 获取处理后的选中节点ID数组
        getProcessedCheckedKeys: function () {
            const treeStore = this.$refs.datatree.store;
            const resultNodes = [];
            // 递归遍历函数
            const traverse = (node) => {
                // 获取当前节点的直接子节点列表
                const childNodes = node.root ? node.root.childNodes : node.childNodes;
                childNodes.forEach(child => {
                    if (child.checked) resultNodes.push(child.data);    // 情况1: 节点被全选
                    else if (child.indeterminate) traverse(child);    // 情况2: 节点处于半选状态
                });
            };
            // 从根store开始遍历
            traverse(treeStore);
            return resultNodes;
        }
    },
    template: `<div class="selectparts">
        <div class="searchbar">
            <el-input placeholder="检索" v-model="search" clearable></el-input>
        </div>
        <el-tree ref="datatree" node-key="Qp_ID" :props="props"  :check-on-click-node="true"
        :data="datas" show-checkbox @check="handleCheckChange" :filter-node-method="filterNode" empty-text="没有满足条件的数据">
            <div class="custom-node" slot-scope="{ node, data }">
                <span class="large" v-html="showsearch(data.Qp_Name,search)"></span>
            </div> 
        </el-tree>
    </div>`
});