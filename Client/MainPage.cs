﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
namespace Client
{
    public partial class MainPage : Form
    {

        private Client client;
        private enum Status { BUYING, SELLING, NONE};
        private Status status = Status.NONE;

        public MainPage(Client client)
        {
            InitializeComponent();
            this.panelMyWallet.Visible = true;
            this.panelSellingOrders.Visible = false;
            this.panelPurchaseOrders.Visible = false;

            this.panel3.Visible = true;
            this.panel4.Visible = false;
            this.panel5.Visible = false;

            this.client = client;



            /* Test *//*
            add_transactions("sold", "1000", "1.2", "12-06-10");
            updateBalance("" + 2000);
            updateQuote("" + 2.0);
            updateDiginotesOwned("" + 200);
            //updateFullName(client.Name());*/
           
        }

        public void inicialize_wallet()
        {
            if (InvokeRequired)                                                                     // I'm not in UI thread
                BeginInvoke((MethodInvoker)delegate { inicialize_wallet(); });
            else { 
                client.UpdateUser();
                updateFullName(client.User.Name);
                double quote = client.GetQuote();
                updateBalance("" + client.User.Money);
                updateQuote("" + quote);
                updateDiginotesOwned("" + client.User.Wallet.Count);

                List<Common.Order> orders_pending = client.GetPendingOrders();
                long aux = 0;
                foreach (Common.Order order in orders_pending)
                {
                    if(order.Type == Common.Order.OrderType.Purchase)
                    {
                        aux += (long)(order.Amount * quote);
                    }
                }
                updateAvailableBalance("" + (client.User.Money - aux));

            }

        }

        public void inicialize_purchase()
        {
            if (InvokeRequired)                                                                     // I'm not in UI thread
                BeginInvoke((MethodInvoker)delegate { inicialize_purchase(); });
            else
            {
                List<Common.Order> orders_pending = client.GetPendingOrders();
                long aux = 0;
                double quote = client.GetQuote();
                dataGridView1.Rows.Clear();
                ArrayList row = new ArrayList();
                foreach (Common.Order order in orders_pending)
                {
                    if (order.Type == Common.Order.OrderType.Purchase)
                    {
                        aux += ((long)(order.Amount) - (long)(order.CurrentAmount));

                        row = new ArrayList();
                        row.Add(order.Id);
                        row.Add(order.Timestamp);
                        row.Add(order.Amount - order.CurrentAmount);

                        if (order.Available > 0)
                            row.Add("Confirm");
                        else
                            row.Add("");
                        row.Add("Delete");
                        dataGridView1.Rows.Add(row.ToArray());

                    }
                }
                if (aux != 0)
                    panel19.Visible = true;
                else
                    panel19.Visible = false;

                updateBuying("" + aux);
                updateQuote("" + quote);
                updateAvailableBalance("" + (client.User.Money - aux * quote));

            }
        }

        public void inicialize_selling()
        {
            if (InvokeRequired)                                                                     // I'm not in UI thread
                BeginInvoke((MethodInvoker)delegate { inicialize_selling(); });
            else
            {
                List<Common.Order> orders_pending = client.GetPendingOrders();
                long aux = 0;
                double quote = client.GetQuote();
                dataGridView2.Rows.Clear();
                ArrayList row = new ArrayList();
                foreach (Common.Order order in orders_pending)
                {
                    if (order.Type == Common.Order.OrderType.Selling)
                    {
                        aux += ((long)(order.Amount) - (long)(order.CurrentAmount));

                        row = new ArrayList();
                        row.Add(order.Id);
                        row.Add(order.Timestamp);
                        row.Add(order.Amount);

                        if (order.Available > 0)
                            row.Add("Confirm");
                        else
                            row.Add("");
                        row.Add("Delete");
                        dataGridView2.Rows.Add(row.ToArray());



                    }
                }
                if (aux != 0)
                    panel1ChangeQuote.Visible = true;
                else
                    panel1ChangeQuote.Visible = false;

                updateSelling("" + aux);
                updateQuote("" + quote);
                updateDiginotesOwned("" + client.User.Wallet.Count);

            }
        }



        private void updateBuying(String amount)
        {
            label34.Text = amount;
        }

        private void updateSelling(String amount)
        {
            currentlySelling.Text = amount;
        }

        private void updateAvailableBalance(string balance)
        {
            label35.Text = balance;
            label22.Text = balance;
        }

        private void updateFullName(string name)
        {
            this.labelFullName.Text = name;
        }

        private void updateDiginotesOwned(string no)
        {
            this.labelDigisOwned.Text = no;
            this.label18.Text = no;
        }

        public void updateQuote(string quote)
        {
            if (InvokeRequired)                                               // I'm not in UI thread
                BeginInvoke((MethodInvoker)delegate { updateQuote(quote); });  // Invoke using an anonymous delegate
            else
            {
                this.labelQuote.Text = quote;
                this.label32.Text = quote;
                this.label6.Text = quote;
            }

            
        }

        private void updateBalance(string balance)
        {
            this.Balance.Text = balance;
        }

     /*   private void add_transactions(string type, string quantity, string quote, string date)
        {

            ListViewItem lvi = new ListViewItem(date);
            lvi.SubItems.Add(type);
            lvi.SubItems.Add(quantity);
            lvi.SubItems.Add(quote);
            this.History.Items.Add(lvi);

        }*/


        private void button3_Click(object sender, EventArgs e)
        {
            client.Logout();
            Application.Exit();
        }

        private void buttonOverview_Click(object sender, EventArgs e)
        {
            erase_messages();
            inicialize_wallet();
            this.panelMyWallet.Visible = true;
            this.panelSellingOrders.Visible = false;
            this.panelPurchaseOrders.Visible = false;

            this.panel3.Visible = true;
            this.panel4.Visible = false;
            this.panel5.Visible = false;

        }

        private void buttonSellingOrder_Click(object sender, EventArgs e)
        {
            if (status != Status.NONE)
                return;
            erase_messages();
            inicialize_selling();
            this.panelMyWallet.Visible = false;
            this.panelSellingOrders.Visible = true;
            this.panelPurchaseOrders.Visible = false;

            this.panel3.Visible = false;
            this.panel4.Visible = true;
            this.panel5.Visible = false;

        }

        private void buttonPurchaseOrder_Click(object sender, EventArgs e)
        {
            if (status != Status.NONE)
                return;
            erase_messages();
            inicialize_purchase();
            this.panelMyWallet.Visible = false;
            this.panelSellingOrders.Visible = false;
            this.panelPurchaseOrders.Visible = true;

            this.panel3.Visible = false;
            this.panel4.Visible = false;
            this.panel5.Visible = true;
        }


        /* New Buy Order */
        private void button4_Click(object sender, EventArgs e)
        {
            // verificar se o texto é válido, se não for, inserir mensagem de erro
            this.panelNewBuyOrder.Visible = false;
            this.panelBuyProgress.Visible = true;
            String input = this.buyAmount.Text;

            if (!input.Equals(""))
            {
                long amount;
                try {
                    amount = Convert.ToInt64(input);
                    if (amount <= 0)
                    {
                        this.put_message_buy_error("Number of diginotes to buy must be greater than 0");
                        this.panelNewBuyOrder.Visible = true;
                        this.panelBuyProgress.Visible = false;
                        return;
                    }
                    Common.Info status = client.AddOrder(Common.Order.OrderType.Purchase, amount);
                    if (status == Common.Info.OrderParciallyCompleted || status == Common.Info.OrderPending)
                    {
                        double currentQuote = client.GetQuote();
                        

                        this.put_message_buy_error("Not All Diginotes were bought. You must define a new quote (higher or equal) than the current quote (" + currentQuote + ")");
                        this.panelNewBuyOrder.Visible = false;
                        this.panelBuyProgress.Visible = false;
                        this.panelDefineQuoteBuy.Visible = true;
                        this.status = Status.BUYING;
                        
                    }
                    else if(status == Common.Info.OrderCompleted)
                    {
                        this.put_message_buy_success("The Order was successufully created");
                        this.panelNewBuyOrder.Visible = true;
                        this.panelBuyProgress.Visible = false;
                    }
                    else
                    {
                        this.put_message_buy_error("Oops, seems like there's not enough money for this transaction");
                        this.panelNewBuyOrder.Visible = true;
                        this.panelBuyProgress.Visible = false;
                    }


                }
                catch(FormatException)
                {
                    this.panelNewBuyOrder.Visible = true;
                    this.panelBuyProgress.Visible = false;
                    this.put_message_buy_error("Insert a valid Amount input");
                }

            }else
            {
                this.panelNewBuyOrder.Visible = true;
                this.panelBuyProgress.Visible = false;
                this.put_message_buy_error("Insert a valid Amount input");
            }


 
        }


        private void put_message_buy_error(String msg)
        {
            this.panelSucessMessBuy.Visible = false;
            this.labelErrorBuy.Text = msg;
            this.panelErrorBuy.Visible = true;
        }

        private void put_message_buy_success(String msg)
        {
            this.panelErrorBuy.Visible = false;
            this.labelSucessBuy.Text = msg;
            this.panelSucessMessBuy.Visible = true;
        }

        private void erase_messages()
        {
            this.panelErrorBuy.Visible = false;
            this.panelSucessMessBuy.Visible = false;
            this.panelSuccessMessage.Visible = false;
            this.panelErrorMessage.Visible = false;
        }

        private void put_message_sell_error(String msg)
        {
            this.panelSuccessMessage.Visible = false;
            this.labelErrorMessSell.Text = msg;
            this.panelErrorMessage.Visible = true;
        }

        private void put_message_sell_success(String msg)
        {
            this.panelErrorMessage.Visible = false;
            this.labelSuccessMessSell.Text = msg;
            this.panelSuccessMessage.Visible = true;
        }

        /* Defines a new quote when a user tries to buy */ 
        private void button5_Click(object sender, EventArgs e)
        {
            String input = bunifuMaterialTextbox9.Text;
            double quote = -1;
            try
            {
                quote = Convert.ToDouble(input);
            }catch(FormatException)
            {
                this.put_message_buy_error("Please select a valid quote");
                return;
            }

            double currentQuote = this.client.GetQuote();

            if (quote < currentQuote)
            {
                this.put_message_buy_error("Defined quote is smaller than the current quote (" + currentQuote + ")");
            }else
            {
                this.panelNewBuyOrder.Visible = true;
                this.panelBuyProgress.Visible = false;
                this.panelDefineQuoteBuy.Visible = false;
                client.AddQuote(quote, Common.Order.OrderType.Purchase);
                this.status = Status.NONE;
                this.put_message_buy_success("Order Successufully created");
                this.inicialize_purchase();

            }

            
        }

        /* Increases balance */
        private void button10_Click(object sender, EventArgs e)
        {
            String input = bunifuMaterialTextbox8.Text;
            long amount = 0;
            amount = Convert.ToInt64(input);

            client.AddMoney(amount);

            // update
            Double aux = Convert.ToDouble(this.Balance.Text);
            aux += amount;
            this.Balance.Text = "" + aux;
            this.bunifuMaterialTextbox8.Text = "";

            aux = Convert.ToDouble(this.label35.Text);
            aux += amount;
            updateAvailableBalance(""  + aux);


        }

        /* Retrieves money */
        private void button11_Click(object sender, EventArgs e)
        {
            String input = bunifuMaterialTextbox12.Text;
            long amount = 0;
            amount = Convert.ToInt64(input);

            client.AddMoney(-amount);

            // update
            double aux = Convert.ToDouble( this.Balance.Text);
            aux -= amount;
            this.Balance.Text = "" + aux;
            this.bunifuMaterialTextbox12.Text = "";

            aux = Convert.ToDouble(this.label35.Text);
            aux -= amount;
            updateAvailableBalance("" + aux);
        }

       


        /* When a user is in purchase orders and whants to update quote */
        private void button9_Click(object sender, EventArgs e)
        {
            
            String input = bunifuMaterialTextbox11.Text;
            double quote = -1;
            try
            {
                quote = Convert.ToDouble(input);
            }
            catch (Exception)
            {
                this.put_message_buy_error("Please select a valid quote");
                return;
            }

            double currentQuote = this.client.GetQuote();

            if (quote < currentQuote)
            {
                this.put_message_buy_error("Defined quote is smaller than the current quote (" + currentQuote + ")");
            }
            else
            {
                client.AddQuote(quote, Common.Order.OrderType.Purchase);
                this.status = Status.NONE;
                this.put_message_buy_success("Quote Successufully updated");
                label32.Text = input;
            }

            this.inicialize_purchase();
        }


        /* New Sell Order */
        private void button1_Click(object sender, EventArgs e)
        {
            // verificar se o texto é válido, se não for, inserir mensagem de erro
            this.panelNewSellOrder.Visible = false;
            this.panelProgress.Visible = true;
            String input = this.bunifuMaterialTextbox1.Text;

            if (!input.Equals(""))
            {
                long amount;
                try
                {
                    amount = Convert.ToInt64(input);
                    if(amount <= 0)
                    {
                        this.put_message_sell_error("Number of diginotes to be sold must be greater than 0");
                        this.panelNewBuyOrder.Visible = true;
                        this.panelBuyProgress.Visible = false;
                        return;
                    }

                    Common.Info status = client.AddOrder(Common.Order.OrderType.Selling, amount);
                    if (status == Common.Info.OrderParciallyCompleted || status == Common.Info.OrderPending)
                    {
                        double currentQuote = client.GetQuote();


                        this.put_message_sell_error("Not All Diginotes were sold. You must define a new quote (smaller or equal) than the current quote (" + currentQuote + ")");
                        this.panelNewSellOrder.Visible = false;
                        this.panelProgress.Visible = false;
                        this.panelSelectQoute.Visible = true;
                        this.status = Status.SELLING;

                    }
                    else if (status == Common.Info.OrderCompleted)
                    {
                        this.put_message_sell_success("The Order was successufully created");
                        this.panelNewSellOrder.Visible = true;
                        this.panelProgress.Visible = false;
                    }
                    else
                    {
                        this.put_message_sell_error("Oops, seems like there's not enough diginotes for this transaction");
                        this.panelNewSellOrder.Visible = true;
                        this.panelProgress.Visible = false;
                    }


                }
                catch (FormatException)
                {
                    this.panelNewSellOrder.Visible = true;
                    this.panelProgress.Visible = false;
                    this.put_message_sell_error("Insert a valid Amount input");
                }

            }
            else
            {
                this.panelNewSellOrder.Visible = true;
                this.panelProgress.Visible = false;
                this.put_message_sell_error("Insert a valid Amount input");
            }
        }

        
        /* When the user tries to sell diginotes but not all were sold and quote must be defined*/
        private void button7_Click(object sender, EventArgs e)
        {
            String input = bunifuMaterialTextbox7.Text;
            double quote = -1;
            try
            {
                quote = Convert.ToDouble(input);
            }
            catch (FormatException)
            {
                this.put_message_sell_error("Please select a valid quote");
                return;
            }

            double currentQuote = this.client.GetQuote();

            if (quote > currentQuote)
            {
                this.put_message_buy_error("Defined quote is bigger than the current quote (" + currentQuote + ")");
            }
            else
            {
                client.AddQuote(quote, Common.Order.OrderType.Selling);
                this.status = Status.NONE;
                this.put_message_buy_success("Order Successufully created");
                this.panelNewSellOrder.Visible = true;
                this.panelProgress.Visible = false;
                this.panelSelectQoute.Visible = false;
                this.inicialize_selling();

            }
        }

        /* update quote at selling tab */
        private void button6_Click(object sender, EventArgs e)
        {
            

             String input = bunifuMaterialTextbox5.Text;
            double quote = -1;
            try
            {
                quote = Convert.ToDouble(input);
            }
            catch (Exception)
            {
                this.put_message_sell_error("Please select a valid quote");
                return;
            }

            double currentQuote = this.client.GetQuote();

            if (quote > currentQuote)
            {
                this.put_message_sell_error("Defined quote is bigger than the current quote (" + currentQuote + ")");
            }
            else
            {
                client.AddQuote(quote, Common.Order.OrderType.Selling);
                this.status = Status.NONE;
                this.put_message_sell_success("Quote Successufully updated");
                label6.Text = input;
            }

            this.inicialize_selling();

        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == 3 && !dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.Equals(""))
            {
                MessageBox.Show("" + e.RowIndex + " wants to be confirmed");

                client.DeleteOrder(Common.Order.OrderType.Selling, Int64.Parse(dataGridView2.Rows[e.RowIndex].Cells[0].Value.ToString()));
                client.AddOrder(Common.Order.OrderType.Selling, Int64.Parse(dataGridView2.Rows[e.RowIndex].Cells[2].Value.ToString()));

                //client.confirmOrder(Common.Order.OrderType.Selling, Int64.Parse(dataGridView2.Rows[e.RowIndex].Cells[0].Value.ToString()));
                inicialize_selling();
                

            }else if(e.ColumnIndex == 4)
            {
                DialogResult dr = MessageBox.Show("Are you sure you want to delete this selling order?\nIf you choose to delete this selling order you can undo it by creating another one", "Are you sure?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                if (dr == DialogResult.Yes)
                {
                    client.DeleteOrder(Common.Order.OrderType.Selling, Int64.Parse(dataGridView2.Rows[e.RowIndex].Cells[0].Value.ToString()));
                    inicialize_selling();
                }
                else if (dr == DialogResult.Cancel || dr == DialogResult.No)
                {
                    return;
                }
                
                //MessageBox.Show("" + dataGridView2.Rows[e.RowIndex].Cells[0].Value + " wants to be " + dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
            }
        }


        public void pendingOrderSuspended(Common.Order.OrderType type, string orderId)
        {
            if (InvokeRequired)                                                                     // I'm not in UI thread
                BeginInvoke((MethodInvoker)delegate { pendingOrderSuspended(type, orderId); });     // Invoke using an anonymous delegate
            else
            {
                if (type == Common.Order.OrderType.Purchase)
                {
                    for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                    {
                        string id = "" + (long)dataGridView1.Rows[i].Cells[0].Value;
                        if (id.Equals(orderId))
                        {
                            dataGridView1.Rows[i].Cells[3].Value = "Confirm";
                            
                        }
                    }
                }
                else if (type == Common.Order.OrderType.Selling)
                {
                     for (int i = 0; i < dataGridView2.Rows.Count - 1; i++)
                    {
                        string id = "" + (long)dataGridView2.Rows[i].Cells[0].Value;
                        if (id.Equals(orderId))
                        {
                            dataGridView2.Rows[i].Cells[3].Value = "Confirm";
                            
                        }
                    } 
                    
                }
            }


        }


        public void pendingOrderNotSuspended(Common.Order.OrderType type, string orderId)
        {
            if (InvokeRequired)                                               // I'm not in UI thread
                BeginInvoke((MethodInvoker)delegate { pendingOrderNotSuspended(type, orderId); });  // Invoke using an anonymous delegate
            else { 
                if (type == Common.Order.OrderType.Purchase)
                {
                    for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                    {
                        string id = "" + (long)dataGridView1.Rows[i].Cells[0].Value;
                        if (id.Equals(orderId))
                        {
                            dataGridView1.Rows[i].Cells[3].Value = "";
                            client.DeleteOrder(Common.Order.OrderType.Purchase, Int64.Parse(dataGridView1.Rows[i].Cells[0].Value.ToString()));
                            client.AddOrder(Common.Order.OrderType.Purchase, Int64.Parse(dataGridView1.Rows[i].Cells[2].Value.ToString()));
                            return;
                        }
                    }
                }
                else if (type == Common.Order.OrderType.Selling)
                {
                    for (int i = 0; i < dataGridView2.Rows.Count - 1; i++)
                    {
                        string id = "" + (long)dataGridView1.Rows[i].Cells[0].Value;
                        if (id.Equals(orderId))
                        {
                            dataGridView2.Rows[i].Cells[3].Value = "";
                            client.DeleteOrder(Common.Order.OrderType.Selling, Int64.Parse(dataGridView2.Rows[i].Cells[0].Value.ToString()));
                            client.AddOrder(Common.Order.OrderType.Selling, Int64.Parse(dataGridView2.Rows[i].Cells[2].Value.ToString()));
                            return;
                        }
                    }
                }

            }
        }

        private void dataGridView1_CellClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3 && !dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.Equals(""))
            {

                client.DeleteOrder(Common.Order.OrderType.Purchase, Int64.Parse(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString()));
                client.AddOrder(Common.Order.OrderType.Purchase, Int64.Parse(dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString()));
                //MessageBox.Show("" + e.RowIndex + " wants to be confirmed");
                //client.confirmOrder(Common.Order.OrderType.Purchase, Int64.Parse(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString()));
                inicialize_purchase();

            }
            else if (e.ColumnIndex == 4)
            {
                DialogResult dr = MessageBox.Show("Are you sure you want to delete this purchase order?\nIf you choose to delete this purchase order you can undo it by creating another one", "Are you sure?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                if (dr == DialogResult.Yes)
                {
                    client.DeleteOrder(Common.Order.OrderType.Purchase, Int64.Parse(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString()));
                    inicialize_purchase();
                }
                else if (dr == DialogResult.Cancel || dr == DialogResult.No)
                {
                    return;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            client.Logout();
            this.Hide();
            Client.form.Show();
        }
    }
}
