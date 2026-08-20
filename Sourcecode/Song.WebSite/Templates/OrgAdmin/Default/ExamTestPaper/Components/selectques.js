//固定试卷在选择试题时的试题行
//事件：
// checked:选中试题的事件，参数：试题，选中状态
// collect:收藏试题的事件，参数：试题，收藏状态
Vue.component('selectques', {
    // 属性
    // ques:试题
    // types:所有题型
    // admin: 当前登录的管理员，收藏的显示
    //serial:试题的序号
    // search: 搜索关键字
    // showsearch: 显示搜索关键字
    // showsearch(text,search){}
    props: ['ques', 'types', 'admin', 'serial', 'search'],
    data: function () {
        return {
            loading: false
        }
    },
    watch: {

    },
    computed: {},
    mounted: function () {

    },
    methods: {
        //解析试题答案
        answer: function (q) {
            let func = 'type' + q.Qus_Type;
            return this[func] != null ? this[func](q) : '';
        },
        type1: q => String.fromCharCode(65 + q.Qus_Items.findIndex(m => m.Ans_IsCorrect)),
        type2: q => q.Qus_Items.map((m, i) => m.Ans_IsCorrect ? String.fromCharCode(65 + i) : null)
            .filter(m => m !== null).join('、'),
        type3: q => q.Qus_IsCorrect ? "正确" : "错误",
        type4: q => q.Qus_Answer,
        type5: q => q.Qus_Items.length === 1 ?
            q.Qus_Items[0].Ans_Context : q.Qus_Items.map((m, i) => `${i + 1}、${m.Ans_Context}`).join('；'),
        //选中的事件
        checked: function (q) {
            this.$emit('checked', q, q.checked);
        },
        //收藏状态的变更
        collect: function (ques, state) {
            this.$emit('collect', ques, state);
        },
        //设置试题的收藏显示状态
        setcollect: function (ques, state) {
            let coll = this.$refs['ques_collect'];
            if (coll != null && ques.Qus_ID == this.ques.Qus_ID) {
                coll.changeshow(ques, state);
            }
        }
    },
    template: ` <div class="selectques" :qid="ques.Qus_ID">
    <div class="quesrow-title">
        <div class="index">{{serial}}.</div>
        <ques_type :type="ques.Qus_Type" :types="types" :showicon="true" :showname="true">
        </ques_type>
        <ques_diff :ques="ques"></ques_diff>
        <ques_collect ref="ques_collect" :ques="ques" :accid="admin.Acc_Id" @change="collect"></ques_collect>
        <div>
            <el-checkbox v-model="ques.checked" @change="checked(ques)">选中</el-checkbox>
        </div>
    </div>
    <div class="quesrow-content" @click="$set(ques,'showdetail',!ques.showdetail)"
        :showdetail="ques.showdetail">
        <span v-html="showsearch(ques.Qus_Title,search)"></span>
        <div v-if="ques.Qus_Type==1 || ques.Qus_Type==2" class="qus_items" remark="单选、多选的选项">
            <div v-for="(item,index) in ques.Qus_Items" :correct="item.Ans_IsCorrect"
                v-html="String.fromCharCode(65 + index) +'.'+item.Ans_Context">
            </div>
        </div>
        <div class="answer">
            正确答案：<span v-html="answer(ques)"></span>
        </div>
    </div>
</div>`
});

