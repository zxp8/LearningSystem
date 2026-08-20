//编辑试题类型的别名
Vue.component('editbyname', {
    props: ['value'],
    data: function () {
        return {
            visible: false,
            typeitem: {},    //试题类型的数据项
            item: {},      //用于编辑的临时类型
            loading: false
        }
    },
    watch: {

    },
    computed: {},
    mounted: function () {

    },
    methods: {
        //显示面板
        show: function (typeitem) {
            this.typeitem = typeitem;
            this.item = $api.clone(typeitem);
            this.visible = true;
        },
        enter: function () {
            this.typeitem.byname = this.item.byname;
            this.visible = false;
        }
    },
    template: `<el-dialog :visible.sync="visible" width="30%" :before-close="()=>{}">
    <span slot="title">修改 “{{item.name}}题” 的别名</span>
    <el-form ref="form" :model="item" label-width="50px">
      <el-form-item label="别名">
        <el-input v-model="item.byname" clearable></el-input>
      </el-form-item>
    </el-form>
    <span slot="footer" class="dialog-footer">
      <el-button @click="visible = false">取 消</el-button>
      <el-button type="primary" @click="enter">确 定</el-button>
    </span>
  </el-dialog>`
});