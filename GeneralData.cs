using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace ExchangeReceiver
{

    public enum OrderType { Sell, Buy, allCancel, Amend };

    public class OrderInfo
    {
        public OrderInfo(double price, int qty, OrderType orderType, string comment)
        {
            this.qty = qty;
            this.price = price;
            this.orderType = orderType;
            this.comment = comment;
        }

        public int qty;
        public double price;
        public OrderType orderType;
        public string comment;
    }



    public class GeneralData
    {

        public OrderBook ob;
        public MatchInfo mi;

        public UInt64 msg_time;
        public double stop_high;
        public double stop_low;
        public double base_price;
        public int decimal_locator;
        //public long close_time;
        public string commodx;
        public int end_date;
        public string ip;
        public int port;
        public int gate_target_pos;
        public int settle_month;


        public int positions
        {
            get
            {
                int rtn = 0;

                foreach (KeyValuePair<double, int> item in Dic_Position)
                {
                    if (item.Key > 0)
                        rtn += item.Value;
                    else
                        rtn -= item.Value;
                }

                return rtn;
            }
        }
        public int orderCount
        {
            get
            {
                int rtn = 0;

                foreach (KeyValuePair<string, OrderInfo> item in Dic_Order)
                {
                    if (item.Value.orderType == OrderType.Buy)
                        rtn += item.Value.qty;
                    else
                        rtn -= item.Value.qty;
                }

                return rtn;
            }
        }

        public ConcurrentDictionary<string, OrderInfo> Dic_Order = new ConcurrentDictionary<string, OrderInfo>();//do not change to  <double, int>, sometimes we only have price or qty
        private ConcurrentDictionary<double, int> Dic_Position = new ConcurrentDictionary<double, int>();

        public void Change_Order(string key, double price)
        {
            Dic_Order.TryGetValue(key, out OrderInfo oi);
            oi.price = price;
        }
        public void Change_Order(string key, int qty)
        {
            Dic_Order.TryGetValue(key, out OrderInfo oi);
            oi.qty = qty;
        }
        public void Add_Order(string key, double price, int qty, OrderType ot, string comment)
        {
            OrderInfo oi = new OrderInfo(price, qty, ot, comment);

            Dic_Order.TryAdd(key, oi);
        }
        public void Remove_Order(string key)
        {
            Dic_Order.TryRemove(key, out OrderInfo temp);
        }
        public void Match_Order(string key, double price, int qty)
        {
            if (!Dic_Order.TryGetValue(key, out OrderInfo oi))
                return;

            oi.qty -= qty;

            //double price = oi.price;


            if (oi.orderType == OrderType.Buy)
            {
                price = Math.Abs(price);
            }
            else
            {
                price = -Math.Abs(price);
            }



            if (Dic_Position.TryGetValue(price, out int temp))
            {
                Dic_Position[price] += qty;
            }
            else
            {
                Dic_Position.TryAdd(price, qty);
            }



            if (oi.qty == 0)
            {
                Dic_Order.TryRemove(key, out OrderInfo temp2);
            }
        }
        public double CalPosPL
        {
            get
            {
                if (ob.time_source == 0 || mi.time_source == 0)
                    return 0;

                double rtn = 0;

                double mp = mi.match_price;

                foreach (KeyValuePair<double, int> kvp in Dic_Position)
                {
                    if (kvp.Key > 0)
                    {
                        rtn += ((mi.match_price - kvp.Key) * kvp.Value);
                    }
                    else
                    {
                        rtn -= ((mi.match_price + kvp.Key) * kvp.Value);
                    }
                }

                return rtn;
            }
        }

        //public double CalTradePL()
        //{
        //    if (ob.time_source == 0 || mi.time_source == 0)
        //        return 0;

        //    double rtn = 0;



        //    return 9999;
        //}

        //public void ADD_Position(double price, int qty, OrderType ot)
        //{
        //    if (ot == OrderType.Buy)
        //    {
        //        price = Math.Abs(price);
        //    }
        //    else
        //    {
        //        price = -Math.Abs(price);
        //    }


        //    if (Dic_Position.TryGetValue(price, out int temp))
        //    {
        //        Dic_Position[price] += qty;
        //    }
        //    else
        //    {
        //        Dic_Position.TryAdd(price, qty);
        //    }
        //}

        public int futures_month_idx;
        public double price_tick; //orderbook price min gap
        public ExchangeType exchange_type;

        public double current_high;
        public double current_low;

        public GeneralData(
            string symbol,
            string commodx,
            int end_date,
            int futures_month_idx,
            //long close_time,
            UInt64 msg_time,
            double base_price,
            double stop_high,
            double stop_low,
            ExchangeType exchange_type,
            double price_tick,
            int decimal_locator,
            int settle_month
            )
        {
            this.settle_month = settle_month;
            this.msg_time = msg_time;
            this.base_price = base_price;
            this.stop_high = stop_high;
            this.stop_low = stop_low;
            this.current_high = base_price;
            this.current_low = base_price;
            //this.close_time = close_time;
            this.price_tick = price_tick;
            this.commodx = commodx;
            this.end_date = end_date;

            this.futures_month_idx = futures_month_idx;

            if (decimal_locator == 0)
            {
                this.decimal_locator = 1;
            }
            else if (symbol.Substring(0, 3) == "TGF")
            {
                this.decimal_locator = 10;
            }
            else
            {
                this.decimal_locator = Convert.ToInt32(Math.Pow(10, 4 - decimal_locator));
            }

            this.exchange_type = exchange_type;
        }
    }
}
