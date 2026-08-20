//关键字的选择，用于条件查询
Vue.component('selecttag', {
    props: ['org'],
    data: function () {
        return {
            query: { "orgid": -1, "couid": 0, "isdeleted": false, "name": '', "size": 10, "index": 1 },
            value: '',
            options: [],
            loading: false,
        }
    },
    watch: {
        "org": {
            handler: function (val) {
                this.query.orgid = val.Org_ID;
                this.remoteMethod();
            }, immediate: true,
        },
        //当选中的值变化时
        "value": {
            handler: function (val) {
                this.$emit('change', val);
                this.remoteMethod();
            }
        }
    },
    methods: {
        //远程搜索
        remoteMethod: function (search) {
            var th = this;
            th.query.name = search;
            $api.get("ExamQues/TagPager", th.query)
                .then(req => {
                    if (req.data.success) {
                        let result = req.data.result;
                        th.options = result;
                    } else {
                        console.error(req.data.exception);
                        throw req.config.way + ' ' + req.data.message;
                    }
                }).catch(err => console.error(err))
                .finally(() => { });
        },
        //清空选择项
        clear: function () {
            this.value = [];
        }
    },
    template: `<div>
        <el-select v-model="value" multiple filterable clearable
        remote reserve-keyword placeholder="请输入关键词" style="width:100%"
        :remote-method="remoteMethod" :loading="loading">
            <el-option v-for="item in options" :key="item.Qtag_ID" :label="item.Qtag_Name" :value="item.Qtag_ID">
            </el-option>
        </el-select>
    </div>`
});