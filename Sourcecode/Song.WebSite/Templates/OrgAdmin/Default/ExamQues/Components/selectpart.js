//试题分类的选择组件
//事件：
//@update:选中的分类对象，选中的分类id
Vue.component('selectpart', {
    props: ['orgid', 'parts'],
    data: function () {
        return {
            datas: [],       //试题分类     
            search: '',      //分类搜索关键字    
            selected: [],   //已选的试题分类
            loading: false,   //分类加载中
        }
    },
    watch: {
        'orgid': {
            handler: function (nv, ov) {
                if (nv == null || nv == ov) return;
                this.getallparts();
            }, immediate: true,
        },
        'parts': {
            handler: function (nv, ov) {
                if (nv == null || nv == ov) return;
                if (this.selected.length > 0) return;
                this.selected = nv;
                this.setCheckedKeys();
            }, immediate: true,
        },
        //过滤树形数据
        'search': function (val) {
            this.$refs.parttree.filter(val);
        },
    },
    computed: {},
    mounted: function () {

    },
    methods: {
        /** 试题分类 */
        //获取所有试题分类
        getallparts: function () {
            var th = this;
            th.loading = true;
            $api.get("ExamQues/PartTreeFront", { "orgid": th.orgid })
                .then(req => {
                    if (req.data.success) {
                        th.datas = req.data.result;
                    } else {
                        console.error(req.data.exception);
                        throw req.config.way + ' ' + req.data.message;
                    }
                }).catch(err => console.error(err))
                .finally(() => th.loading = false);
        },
        //过滤树形
        filterNode: function (value, data) {
            if (!value) return true;
            var txt = $api.trim(value.toLowerCase());
            if (txt == '') return true;
            return data.Qp_Name.toLowerCase().indexOf(txt) !== -1;
        },
        //分类的节点点击事件
        //data:当前节点对象，即QuesPart实体
        //stat:树形节点状态，checkedNodes为选中节点，halfCheckedNodes为半选中节点
        partcheck: function (data, stat) {
            let checkedNodes = this.$refs.parttree.getCheckedNodes();
            this.selected.splice(0, this.selected.length);
            this.selected = [];
            for (let i = 0; i < checkedNodes.length; i++) {
                let part = checkedNodes[i];
                if (!this.selected.some(p => p.Qp_ID == part.Qp_ID)) {
                    if (part.children == null) this.selected.push(part);
                }
            }
            //触发事件
            let partid = this.selected.map(item => item.Qp_ID).join(',');
            this.$emit('update', this.selected, partid);
        },
        //节点点击事件
        handleNodeClick: function (data, node, component) {
            // 判断当前节点是否有子节点
            const hasChildren = node.childNodes && node.childNodes.length > 0;
            if (!hasChildren) {
                const tree = this.$refs.parttree;
                const isChecked = tree.getCheckedKeys().includes(node.key);
                tree.setChecked(node.key, !isChecked);
                this.partcheck(data, null);
            }
        },
        //移除已经选择的分类
        removepart: function (idx) {
            if (idx == -1) this.selected.splice(0, this.selected.length);
            else this.selected.splice(idx, 1);
            this.setCheckedKeys();
            this.partcheck(null, null);
        },
        //设置当前试题的分类
        setCheckedKeys: function () {
            var arr = [];
            for (var i = 0; i < this.selected.length; i++)
                arr.push(this.selected[i].Qp_ID);
            if (this.$refs.parttree)
                this.$refs.parttree.setCheckedKeys(arr);
        },
    },
    template: `<div class="selectpart">
        <div class="parttree">
            <el-input placeholder="检索" v-model="search" clearable></el-input>
            <div class="part_tree">
                <el-tree ref="parttree" node-key="Qp_ID" :props="{label: 'Qp_Name',id:'Qp_ID',children: 'children'}" 
                @node-click="handleNodeClick"
                :data="datas" show-checkbox @check="partcheck" :filter-node-method="filterNode" empty-text="没有满足条件的数据">
                    <div class="custom-node" slot-scope="{ node, data }">
                        <span class="large" v-html="showsearch(data.Qp_Name,search)"></span>
                    </div> 
                </el-tree>
            </div>
        </div>
        <div class="selected_parts">
            <div class="title">
                <span>已选 {{selected?.length ?? 0}} 个分类</span>
                <el-popconfirm confirm-button-text='是的' v-if="selected?.length>0" cancel-button-text='不用' icon="el-icon-info"
                icon-color="red" title="是否清空，确定吗？" @confirm="removepart(-1)">
                    <el-link type="warning" slot="reference"><icon>&#xe800</icon>清空</el-link>
                </el-popconfirm>               
            </div>
            <div class="part_list" v-if="selected?.length>0">
                <el-tag size="medium" v-for="(p,idx) in selected" closable @close="removepart(idx)">
                {{p.Qp_Name}}</el-tag>
            </div>
            <icon null v-else>暂无</icon>
        </div>
</div>`
});