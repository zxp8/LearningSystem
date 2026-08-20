
//固定试卷的试题显示
Vue.component('quesrow', {
    //ques:试题对象
    //index:试题的序号
    //typeidx:试题类型的序号
    //qtypeitems: 所有试题类型的信息
    props: ['ques', 'index', 'typeidx', 'qtypeitems'],
    data: function () {
        return {
            loading: false
        }
    },
    watch: {
        'ques': {
            handler: function (nv, ov) {
                //if ($api.getType(nv.Qus_Items) == "String")
                //this.ques = window.ques.parseAnswer(nv);
            }, immediate: true
        }
    },
    computed: {
        //序号
        serial: function () {
            let init_idx = 0;   //初始序号
            let idx = Number($api.clone(this.typeidx)) - 1;
            while (idx >= 0) {
                init_idx += this.qtypeitems[idx].ques.length;
                idx--;
            }
            return init_idx + this.index + 1;
        }
    },
    mounted: function () {

    },
    methods: {
        //移动题型顺序
        quesmove: function (index, direction) {
            //console.error(index);
            let item = this.qtypeitems[this.typeidx];
            const newArr = [...item.ques];
            const element = newArr.splice(index, 1)[0];
            newArr.splice(index + direction, 0, element);
            item.ques = newArr;
        },
        quesdelete: function (index) {
            let item = this.qtypeitems[this.typeidx];
            item.ques.splice(index, 1);
            item.count=item.ques.length;
        }
    },
    template: `<div class="quesrow" :qid="ques.Qus_ID">
    <div class="queshead">
        <span class="serial">{{serial}}.</span> 
        <span v-html="ques.Qus_Title" class="title"></span>
    </div>
    <div class="btns">
        <el-link icon="el-icon-arrow-up" type="primary" :disabled="index==0"
            @click="quesmove(index,-1)">上移</el-link>
        <el-link icon="el-icon-arrow-down" type="primary" :disabled="index>=qtypeitems[typeidx].ques.length-1"
            @click="quesmove(index,1)">下移</el-link>
        <el-popconfirm title="是否移除该试题？"  @confirm="quesdelete(index)">
            <el-link icon="el-icon-delete" slot="reference" type="warning">移除</el-link>
        </el-popconfirm>            
    </div>
    <div class="quescontent">        
        <div v-if="ques.Qus_Type==1 || ques.Qus_Type==2" class="qus_items" remark="单选、多选的选项">
            <div v-for="(item,index) in ques.Qus_Items" :correct="item.Ans_IsCorrect"
                v-html="String.fromCharCode(65 + index) +'.'+item.Ans_Context">
            </div>
        </div>      
    </div>
</div> `
});