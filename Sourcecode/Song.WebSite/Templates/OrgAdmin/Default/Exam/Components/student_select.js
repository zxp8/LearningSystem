Vue.component('student_select', {
    //theme:考试主题
    props: ['theme', 'orgid'],
    data: function () {
        return {
            examaccounts: [],        //当前考试已经关联的所有学员
            loading: true
        }
    },
    watch: {
        'orgid': {
            handler: function (nv, ov) {
                //this.getStudentAll();
            }, immediate: true
        },
        'theme': {
            handler: function (nv, ov) {
                if (!nv) return;
                this.getSelectedStudent();
            }, immediate: true
        },
    },
    computed: {},
    mounted: function () {

    },
    methods: {
        //考试关联的所有学员
        getSelectedStudent: function () {
            var th = this;
            th.loading = true;
            $api.get("Exam/ScopeAccounts", { "uid": th.theme.Exam_UID })
                .then(req => {
                    if (req.data.success) {
                        th.examaccounts = req.data.result;
                        th.select_event( th.examaccounts);
                    } else {
                        console.error(req.data.exception);
                        throw req.config.way + ' ' + req.data.message;
                    }
                }).catch(err => console.error(err))
                .finally(() => th.$nextTick(() => th.loading = false));
        },
        //选中事件
        //accounts:参数为选中的学员数组
        select_event: function (accounts) {
            for (let j = 0; j < accounts.length; j++) {
                const exists = this.examaccounts.some(user => user.Ac_ID === accounts[j].Ac_ID);
                if (!exists) this.examaccounts.unshift(accounts[j]);
            }
            //生成学员账号ID的数组
            let accids = [];
            for (let i = 0; i < this.examaccounts.length; i++) {
                accids.push(this.examaccounts[i].Ac_ID);
            }
            //生成关联对象
            var relationships = [];
            for (let i = 0; i < this.examaccounts.length; i++) {
                relationships.push({
                    Exam_UID: this.theme.Exam_UID,
                    Ac_ID: this.examaccounts[i].Ac_ID
                });
            }
            this.$emit('selected', accids, this.examaccounts, relationships);
        },
        //移除
        remove: function (account) {
            this.examaccounts = this.examaccounts.filter(function (item) {
                return item.Ac_ID != account.Ac_ID;
            });
            //生成学员账号ID的数组
            let accids = [];
            for (let i = 0; i < this.examaccounts.length; i++) {
                accids.push(this.examaccounts[i].Ac_ID);
            }
            //生成关联对象
            var relationships = [];
            for (let i = 0; i < this.examaccounts.length; i++) {
                relationships.push({
                    Exam_UID: this.theme.Exam_UID,
                    Ac_ID: this.examaccounts[i].Ac_ID
                });
            }
            this.$emit('selected', accids, this.examaccounts, relationships);
        },
        //移除所有
        removeAll: function () {
            this.examaccounts = [];
            this.$emit('selected', [], [], []);
        }
    },
    template: `<div class="student_select">
        <accountselect :orgid="org.Org_ID" @select="select_event" :accounts="examaccounts"></accountselect>
        <el-card shadow="never" class="examaccounts">
            <template slot="header">
               <icon student>已选择的学员</icon>
               <el-popconfirm title="是否移除所有？" @confirm="removeAll">
                    <el-link type="danger"  slot="reference" :disabled="loading" title="移除所有">移除所有</el-link>
                </el-popconfirm>              
            </template>
            <loading v-if="loading">加载中...</loading>
            <el-empty v-else-if="examaccounts.length<1" description="没有指定参加考试的学员"></el-empty>  
            <dl class="list" v-else>
                <dd v-for="(item,i) in examaccounts" :index="i+1"> 
                    <icon class="name" size="large" :woman="item.Ac_Gender==2" :man="item.Ac_Gender==1" v-html='item.Ac_Name'></icon>  
                    <span class="accname" small v-html='item.Ac_AccName'></span>                    
                    <span class="phone"  v-html='item.Ac_MobiTel1'></span>
                    <span class="idcard"  v-html='item.Ac_IDCardNumber'></span>                  
                    <el-link class="btn" type="warning" @click="remove(item)" title="移除">移除</el-link>
                </dd>
            </dl>
            
        </el-card>       
    </div>`
});