using NHibernate.Classic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;

namespace Domain
{


    /// <summary>
    /// 商品
    ///什么是持久化类
    ///持久化就是把数据（内存中的对象）保存到可永久保存的存储设备中（磁盘），持久化类就是持久化数据的载体，
    ///在应用程序中，实现业务的实体类就是持久化类，如我们用到Product类

    ///持久化类的规则
    ///•实现一个默认的（即无参数的）构造方法（constructor）
    ///•提供一个标识属性（identifier property）（可选）
    ///•使用非密封（non-sealed）类以及虚方法（virtual methods） (可选)
    ///
    /// 持久化类可以实现接口
    /// 1、回掉（ILifecycle），可以让持久化对象在Load之后，或者在Delete或Save之前进行必要的初始化与清除步骤
    /// 奇怪的是OnUpdate()回调没有工作，这是因为在NHibernate 1.x 以后的版本，ILifecycle接口已经不建议使用了。
    /// 
    /// 2、合法性验证（IValidatable）回调是在持久化类在保存其持久化状态前进行合法性检查的接口。
    /// 
    /// 总结：目前NHibernate 3.0版本中不建议使用ILifecycle和IValidatable接口。因为这样NHibernate框架就会对持久化类产生侵入性。
    /// </summary>

    public class Product:ILifecycle,IValidatable
    {
        /// <summary>
        /// ID
        /// </summary>
        public virtual int ID { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public virtual string Code { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public virtual string QuantityPerUnit { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public virtual string Unit { get; set; }

        /// <summary>
        /// 售价
        /// </summary>
        public virtual decimal SellPrice { get; set; }

        /// <summary>
        /// 进价
        /// </summary>
        public virtual decimal BuyPrice { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; set; }


        /// <summary>
        /// 在对象即将被save或者insert的时候回调 
        /// LifecycleVeto.Veto是取消操作  如果其中抛出了CallbackException异常，操作被取消，这个异常会被继续传递给应用程序
        /// LifecycleVeto.NoVeto是接受操作
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public virtual LifecycleVeto OnSave(ISession s)
        {
            Console.WriteLine("您调用了Save()方法！");

            return LifecycleVeto.NoVeto;
        }

        /// <summary>
        ///  在对象即将被update的时候回调（也就是对象被传递给ISession.Update()的时候)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public virtual LifecycleVeto OnUpdate(ISession s)
        {
            Console.WriteLine("您调用了Update()方法！");

            return LifecycleVeto.NoVeto;
        }

        /// <summary>
        /// 在对象即将被delete的时候回调
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public virtual LifecycleVeto OnDelete(ISession s)
        {
            Console.WriteLine("您调用了Delete()方法！");

            return LifecycleVeto.NoVeto;
        }

        /// <summary>
        /// 在对象即将被load的时候回调
        /// </summary>
        /// <param name="s"></param>
        /// <param name="id"></param>
        public virtual void OnLoad(ISession s, object id)
        {
            Console.WriteLine("您调用了Load()方法！");
        }

        public virtual void Validate()
        {

            if (this.BuyPrice >= 12M)
            {
                throw new ValidationFailure("进货价格太高，无法受理！");
            }
        }
    }
}
