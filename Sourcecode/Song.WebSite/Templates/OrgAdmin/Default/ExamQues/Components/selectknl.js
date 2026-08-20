//试题编辑中的知识点选择
//事件：
//@update:选中的分类对象，选中的分类id
Vue.component('selectknl', {
    props: ["orgid", "knls"],
    data: function () {
        return {
            datas: [],
            search: '',      //分类搜索知识点 

            selected: [],   //已选的知识点
            loading: false,

        }
    },
    watch: {
        'orgid': {
            handler: function (nv, ov) {
                if (nv == null || nv == ov) return;
                this.getknlall();
            }, immediate: true,
        },
        'knls': {
            handler: function (nv, ov) {
                if (nv == null || nv == ov) return;
                if (this.selected.length > 0) return;
                this.selected = nv;
                this.calcknls(this.datas, nv);
            }, immediate: true,
        },
        //过滤树形数据
        'search': function (val) {
            this.$refs.knltree.filter(val);
        },
    },
    computed: {
        //知识点数量
        knlslength: t => t.selected?.length ?? 0,
    },
    mounted: function () {

    },
    methods: {
        //获取所有知识点列表
        getknlall: function () {
            var th = this;
            th.loading = true;
            $api.get("ExamQues/KnlTree", { "orgid": th.orgid, "search": "", "isuse": true })
                .then(req => {
                    if (req.data.success) {
                        let results = req.data.result;
                        th.calcSerial(results, null, '');
                        //console.error(results);
                        th.datas = results;
                        if (th.selected && th.selected.length > 0)
                            th.datas = th.calcknls(results, th.selected);
                    } else {
                        console.error(req.data.exception);
                        throw req.config.way + ' ' + req.data.message;
                    }
                }).catch(err => console.error(err))
                .finally(() => th.loading = false);
        },
        //计算已经选中的知识点
        calcknls: function (treedata, selectarr) {
            let treedatalen = treedata?.length ?? 0;
            if (treedata == null || treedatalen < 1) return treedata;
            for (let i = 0; i < treedatalen; i++) {
                let idx = selectarr.findIndex(x => x.Qk_ID == treedata[i].Qk_ID);
                treedata[i]["selected"] = idx >= 0;
                if (treedata[i]["children"] != null) {
                    treedata[i].children = this.calcknls(treedata[i].children, selectarr);
                }
            }
            return treedata;
        },
        //计算章节序号
        calcSerial: function (datas, list, lvl) {
            var childarr = list == null ? datas : (list.children ? list.children : null);
            if (childarr == null) return null;
            for (let i = 0; i < childarr.length; i++) {
                childarr[i].serial = lvl + (i + 1) + '.';
                this.calcSerial(datas, childarr[i], childarr[i].serial);
            }
            return list;
        },
        //过滤树形
        filterNode: function (value, data) {
            if (!value) return true;
            var txt = $api.trim(value.toLowerCase());
            if (txt == '') return true;
            return data.Qk_Name.toLowerCase().indexOf(txt) !== -1;
        },
        //分类的节点点击事件
        //data:当前节点对象，即QuesKnowledge实体      
        knlcheck: function (value, ctrl) {
            this.$set(value, "selected", !value.selected);
            let idx = this.selected.findIndex(x => x.Qk_ID == value.Qk_ID);
            if (value.selected && idx < 0) this.selected.push(value);
            if (!value.selected && idx >= 0) this.selected.splice(idx, 1);

            //触发事件
            let knlid = this.selected.map(item => item.Qk_ID).join(',');
            this.$emit('update', this.selected, knlid);
        },
        //获取已经选择的知识点
        gettreeselected: function (selected, selectarr) {
            if (selectarr == null) selectarr = [];
            if (selected == null || this.knlslength < 1) return selectarr;
            for (let i = 0; i < this.knlslength; i++) {
                if (selected[i].selected) selectarr.push(selected[i]);
                if (selected[i]["children"] != null)
                    selectarr = selectarr.concat(this.gettreeselected(selected[i].children));
            }
            return selectarr;
        },
        //移除已经选择的知识点
        removeknl: function (idx) {
            if (idx == -1) this.selected.splice(0, this.selected.length);
            else this.selected.splice(idx, 1);
         
            this.calcknls(this.datas, this.selected);

            //触发事件
            let knlid = this.selected.map(item => item.Qk_ID).join(',');
            this.$emit('update', this.selected, knlid);
        },
    },
    template: `<div class="selectknl">
        <el-card shadow="never" class="knl-tree">
            <template slot="header">               
                <el-input placeholder="检索" v-model="search" clearable></el-input>
            </template>
            <div class="knltree">
                <el-tree ref="knltree" node-key="Qk_ID" :props="{label: 'Qk_Name',id:'Qk_ID',children: 'children'}" 
                :data="datas" @node-click="knlcheck" :filter-node-method="filterNode" empty-text="没有满足条件的数据">
                    <div :class="{'selected':data.selected,'knlnode':true}" slot-scope="{ node, data }">                       
                        <span v-if="data.selected"><icon>&#xa048</icon></span>
                        <span v-html="data.serial"></span>
                        <span class="large" v-html="showsearch(data.Qk_Name,search)"></span>
                    </div> 
                </el-tree>
            </div>
        </el-card>
        <el-card shadow="never" class="selected_knls">
            <div slot="header" class="title">
                <el-badge :value="knlslength" :hidden="knlslength<1">
                    <icon>&#xa029</icon>关联的知识点</span>
                </el-badge>         
                <el-popconfirm confirm-button-text='是的' v-if="selected?.length>0" cancel-button-text='不用' icon="el-icon-info"
                icon-color="red" title="是否清空，确定吗？" @confirm="removeknl(-1)">
                    <el-link type="warning" slot="reference"><icon>&#xe800</icon>清空</el-link>
                </el-popconfirm>                  
            </div>
            <div class="knls_list" v-if="knlslength>0">
                <div v-for="(k,idx) in selected" >
                    {{idx+1}} .                  
                    <el-tag size="medium"closable @close="removeknl(idx)">
                     {{k.Qk_Name}}</el-tag>
                </div>
            </div>
            <div class="knls_list_null" v-else>暂无</div>
        </el-card>    
    </div> `
});