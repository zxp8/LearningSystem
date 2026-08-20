//试题分数
Vue.component('quesscores', {
    //item:试题类型的配置项
    //typeidx:试题类型的序号
    //qtypeitems: 所有试题类型的信息
    //rowcount: 每行显示的试题数量
    props: ['item', 'typeidx', 'qtypeitems', 'rowcount'],
    data: function () {
        return {
            list: [],
            loading: false
        }
    },
    watch: {
        'item': {
            handler: function (val) {
                let [count, number] = [val.count, val.number];
                if (count == 0) count = val.ques == null ? 0 : val.ques.length;
                this.list = [];
                if (count == 0) this.list = [];
                else {
                    let num = Math.floor(number / count * 100) / 100;
                    let tmtotal = 0;  //题型总分，计算所得
                    for (let i = 0; i < count; i++) {
                        tmtotal += num;
                        this.list.push(num);
                    }
                    if (tmtotal != number) {
                        let last = this.list[this.list.length - 1];
                        this.list[this.list.length - 1] = Math.floor((last + number - tmtotal) * 100) / 100;
                    }
                }
                //console.error(this.list);
            }, immediate: true, deep: true
        }
    },
    computed: {
        //临时数据
        'data': t => [t.count, t.number],
        //每行显示的数量
        'row_count': t => t.rowcount == null ? 4 : t.rowcount,
    },
    mounted: function () {

    },
    methods: {
        //试题序号
        serial: function (index) {
            let init_idx = 0;   //初始序号
            let idx = Number($api.clone(this.typeidx)) - 1;
            while (idx >= 0) {
                init_idx += this.qtypeitems[idx].count;
                idx--;
            }
            return init_idx + index + 1;
        },
        //元素的样式
        divstyle: function (idx) {
            let style = 'flex: 0 0 calc(' + (100 / this.row_count) + '% - 10px);';
            let margin = 'margin-right:0px;';
            style += (idx+1) % this.row_count == 0 ? margin : '';
            return style;
        },
    },
    template: `<<div class="quesscores">
        <div v-for="(num,idx) in list" :zero="num<=0" :style="divstyle(idx)">
            <span>{{serial(idx)}}</span>
            <span :zero="num<=0"><b>{{num}}</b> 分</span>
        </div>
    </div>`
});