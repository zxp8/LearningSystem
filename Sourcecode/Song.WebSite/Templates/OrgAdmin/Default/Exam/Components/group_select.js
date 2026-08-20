// 考试编辑中的学员组选择
Vue.component('group_select', {
    //theme:考试主题
    props: ['theme', 'org'],
    data: function () {
        return {
            //按分组选择参考人员
            allsorts: [],    //所有学员组
            selectedsortid: [],       //选中的学员组,记录的是id，不是对象        

            completed: 2,        //是否加载完成，每加载一个条件完成，减一，等于0时为完成

            loading: false
        }
    },
    watch: {
        'org': {
            handler: function (nv, ov) {
                this.getStudentSort();
            }, immediate: true
        },
        'theme': {
            handler: function (nv, ov) {
                this.getSelectedSort();
            }, immediate: true
        },
        //每加载一个条件完成，减一，等于0时为完成
        'completed': function (nv, ov) {
            if (nv == 0) this.selectedObj(this.selectedsortid);
        }

    },
    computed: {},
    mounted: function () {

    },
    methods: {
        //获取所有学员组
        getStudentSort: function () {
            var th = this;
            $api.get('Account/SortAll', { 'orgid': th.org.Org_ID, 'use': true }).then(function (req) {
                if (req.data.success) {
                    th.allsorts = req.data.result;
                } else {
                    console.error(req.data.exception);
                    throw req.config.way + ' ' + req.data.message;
                }
            }).catch(err => console.error(err)).finally(() => th.completed--);
        },
        //获取当前选中的学员组
        getSelectedSort: function () {
            var th = this;
            if (th.theme.Exam_ID <= 0) return;
            $api.get('Exam/ScopeGroups', { 'uid': th.theme.Exam_UID }).then(function (req) {
                if (req.data.success) {
                    var result = req.data.result;
                    for (var i = 0; i < result.length; i++)
                        th.selectedsortid.push(result[i].Sts_ID);
                } else {
                    console.error(req.data.exception);
                    throw req.config.way + ' ' + req.data.message;
                }
            }).catch(err => console.error(err)).finally(() => th.completed--);
        },
        //选中的对象
        selectedObj: function (sortsid) {
            var arr = [];
            //如果没有学员组，或没有选中的学员组，则清空
            if (this.allsorts.length <= 0 || sortsid.length <= 0) {
                this.$emit('selected', [], [], []);
                return arr;
            }
            //如果有选中的学员组
            for (let i = 0; i < sortsid.length; i++) {
                const id = sortsid[i];
                var sort = this.allsorts.find(function (item) {
                    return item.Sts_ID == id;
                });
                if (sort == null) continue;
                arr.push(sort);
            }
            //生成关联对象
            var groups = [];
            for (let i = 0; i < arr.length; i++) {
                const item = arr[i];
                groups.push({
                    Exam_UID: this.theme.Exam_UID,
                    Eg_Type: 2,
                    Org_ID: this.theme.Org_ID,
                    Sts_ID: item.Sts_ID
                });
            }
            this.$emit('selected', sortsid, arr, groups);
            return arr;
        },

    },
    //
    template: `<div class="SortSelected">
        <el-transfer v-model="selectedsortid" :props="{key: 'Sts_ID',label: 'Sts_Name'}" filterable
        :titles="['学员组', '已选择的学员组']" :data="allsorts" @change="selectedObj">
            <span slot-scope="{ option }">
                {{ option.Sts_Name }} <span class="stscount">({{ option.Sts_Count }})</span>
            </span>
        </el-transfer>
    </div>`
});