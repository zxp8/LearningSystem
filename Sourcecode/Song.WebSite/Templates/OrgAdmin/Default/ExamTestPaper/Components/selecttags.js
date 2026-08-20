//关键字的选择
Vue.component('selecttags', {
    props: ['orgid'],
    data: function () {
        return {
            search: '',  //检索字符串    
            datas: [],      //试题关键字数据    

            //关键字的查询条件
            form: { "orgid": "", "couid": "", "isdeleted": false, "name": "", "size": 100, "index": 1 },
            total: 1, //总记录数
            totalpages: 1, //总页数      

            //选中的项
            selecteditems: [],

            loading: false
        }
    },
    watch: {
        'orgid': {
            handler: function (nv, ov) {
                this.form.orgid = nv;
                this.getdatas().then(function ([th, data]) {
                    th.$emit('load', data);
                });
            }, immediate: true, deep: true
        },
    },
    computed: {},
    mounted: function () {

    },
    methods: {
        //所取试题关键字的数据
        getdatas: function (index) {
            var th = this;
            if (index != null) this.form.index = index;
            return new Promise(function (resolve, reject) {
                th.loading = true;
                $api.get("ExamQues/TagPager", th.form)
                    .then(function (req) {
                        if (req.data.success) {
                            var result = req.data.result;
                            for (let i = 0; i < result.length; i++) {
                                //是否选中
                                result[i]["checked"] = th.selecteditems.some(m => m.Qtag_ID == result[i].Qtag_ID);
                            }
                            th.datas = result;
                            th.totalpages = Number(req.data.totalpages);
                            th.total = req.data.total;
                            resolve([th, th.datas]);
                        } else {
                            throw req.data.message;
                        }
                    }).catch(err => console.error(err))
                    .finally(() => th.loading = false);
            });
        },
        //选择关键字
        selecttag: function (item) {
            let arr = this.datas.filter(a => a.checked == true);
            this.selecteditems = [];
            this.$emit('select', arr);
        },
    },
    template: `<div class="selecttags">
    <div  class="searchbar">
        <el-form class="searchbar" ref="form" :inline="true" :model="form" 
        @keyup.enter.native="getdatas" 
        @submit.native.prevent label-width="80px">
            <el-form-item label="">
                <el-input placeholder="检索" v-model="form.name" clearable></el-input>
            </el-form-item>
            <el-form-item label="">
                <el-button type="primary" :loading="loading" @click="getdatas">查询</el-button>
            </el-form-item>
        </el-form>
    </div>
    <loading v-if="loading"></loading>
    <div class="tags">
        <el-checkbox v-for="(item,idx) in datas"  
        v-model="item.checked" @change="selecttag" 
        :label="item.Qtag_Name" border size="medium">
            <span class="large" v-html="showsearch(item.Qtag_Name,form.name)"></span>
        </el-checkbox>             
    </div>
</div>`
});