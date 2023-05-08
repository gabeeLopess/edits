using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace FrmEditora
{
	public partial class FrmEditora : Form
	{
		SqlConnection conexao;
		SqlCommand comando;
		SqlDataAdapter da;
		string strSQL;
		// Variáveis que controlam o ID editora

		private int ultimoID;
		private int proximoID;
        // Variável para minhasecao
        private bool secaoCadastrada;
        public FrmEditora()
		{
			InitializeComponent();
			edit_Load();
		}

		private SqlConnection CriarConexao()
		{
			conexao = new SqlConnection("Data Source=localhost\\SQLEXPRESS;Initial Catalog=Treinamento;Integrated Security=True");
			return conexao;
		}

        private void AtualizarBotoesExcluir()
        {
            if (secaoCadastrada)
            {
                // Se a seção estiver cadastrada, mostrar o botão de excluir
                btnExcluir.Visible = true;
            }
            else
            {
                // Se a seção não estiver cadastrada, ocultar o botão de excluir
                btnExcluir.Visible = false;
            }
        }
        private void btnSalvar_Click_1(object sender, EventArgs e)
		{

		
		conexao = CriarConexao();

			try
			{
                //uso a variavel para armazenar meu id que seria o codEditora
                int id = Convert.ToInt32(txtCodigo.Text);


				if (id > ultimoID)
				{


					strSQL = "INSERT INTO MvtBIBEditora (NOME) VALUES (@NOME)";
					comando = new SqlCommand(strSQL, conexao);
					comando.Parameters.AddWithValue("@NOME", txtNome.Text);
					conexao.Open();
					comando.ExecuteNonQuery();

					MessageBox.Show("Salvo com sucesso");

                    //passando o metodo de obter o ultimo id a variavel
                    ultimoID = ObterUltimoID();
                    //passando um valor a mais para minha variavel id que ira ser igual a minha lbl,
                    //ent agr em minha lbl se eu tinha 1 la, agr irei ter 2, e assim vai
                    proximoID = ultimoID + 1;
					id++;
                    txtCodigo.Text = id.ToString();
                    //limpando
                    txtNome.Text = string.Empty;


					edit_Load();
				}
				else
				{
                    //como ja existe, ira fazer update, dessa forma atualizando.
                    strSQL = "UPDATE MvtBIBEditora  SET NOME =  @NOME WHERE codEditora = @CodEditora";
					comando = new SqlCommand(strSQL, conexao);
					comando.Parameters.AddWithValue("@NOME", txtNome.Text);
					comando.Parameters.AddWithValue("@CodEditora", txtCodigo.Text);
					conexao.Open();
					comando.ExecuteNonQuery();

					MessageBox.Show("Atualizado com sucesso");
                    //passando um valor a mais para minha variavel id que ira ser igual a minha lbl,
                    //isso funciona tb para atualizar, se eu atualizei o id 1, agr ira aparecer o id 2 em minha lbl
                    id++;
                    //limpando
                    txtCodigo.Text = id.ToString();
					txtNome.Text = string.Empty;
                    secaoCadastrada = false; // Inicialmente, a seção não foi cadastrada entao escondo meu botao
                     //metodo atualizar btn excluir						
                    AtualizarBotoesExcluir();
                    edit_Load();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				conexao.Close();

			}

		}

		private void btnExcluir_Click_1(object sender, EventArgs e)
		{

		conexao = CriarConexao();

			try
			{
				int id = Convert.ToInt32(txtCodigo.Text);

				strSQL = "DELETE MvtBIBEditora WHERE codEditora = @CodEditora";
				comando = new SqlCommand(strSQL, conexao);
				comando.Parameters.AddWithValue("@CodEditora", txtCodigo.Text);
				conexao.Open();
				comando.ExecuteNonQuery();

				MessageBox.Show("Excluido com sucesso");

                //mando para minha lbl o proximo id que deveras ser cadastrado
                txtCodigo.Text = proximoID.ToString();
                txtNome.Text = string.Empty;
                secaoCadastrada = false; //Inicialmente, a seção não foi cadastrada entao escondo meu botao
                //metodo atualizar btn excluir	
                AtualizarBotoesExcluir();
                edit_Load();

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				conexao.Close();
				comando.Clone();

			}


		}

		private void edit_Load()
		{
			conexao = CriarConexao();

			try
			{
				strSQL = "SELECT codEditora AS ID, nome AS NOME FROM MvtBIBEditora";


				DataSet ds = new DataSet();
				da = new SqlDataAdapter(strSQL, conexao);

				conexao.Open();
				da.Fill(ds);
				gridLayout.DataSource = ds.Tables[0];

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

		}

		private void FrmEditora_Load(object sender, EventArgs e)
		{
			//conexão com o banco de dados
			conexao = CriarConexao();
			conexao.Open();
            //passando o metodo de obter o ultimo id para uma variavel
            ultimoID = ObterUltimoID();
            //uso essa varaivel para obter o proximo id, usando a ultima variavel + 1
            proximoID = ultimoID + 1;
            //passo isso a minha label
            txtCodigo.Text = proximoID.ToString();
            secaoCadastrada = false; // Inicialmente, a seção não foi cadastrada entao escondo meu botao
            //metodo atualizar btn excluir
            AtualizarBotoesExcluir();

            conexao.Close();
		}

		private int ObterUltimoID()
		{
            //comando SQL  que me permite obter o maior valor presente na coluna codEditora
            string query = "SELECT MAX(codEditora) FROM MvtBIBEditora";
			using (SqlCommand command = new SqlCommand(query, conexao))
			{

				object result = command.ExecuteScalar();
                //verifica se o resultado da consulta SQL é diferente de um id inexistente
                if (result != DBNull.Value)
				{
					return Convert.ToInt32(result);
				}
				else
				{
					return 0;
				}
			}
		}

		private void gridLayout_DoubleClick(object sender, EventArgs e)
		{

		
			try
			{

				if (gridLayout.SelectedRows.Count >= 0)
				{
                    //aqui é o unico lugar onde irei mostrar meu botao excluir,
                    //pois para eu poder dar double click devo ter algum registro cadastrado
                    txtCodigo.Text = gridLayout.SelectedRows[0].Cells[0].Value.ToString();
					txtNome.Text = gridLayout.SelectedRows[0].Cells[1].Value.ToString();
                    btnExcluir.Visible = true;

                }

			}
			catch (Exception ex)
			{

			}
		}
		
		private void label1_Click(object sender, EventArgs e)
		{

		}

		private void label1_Click_1(object sender, EventArgs e)
		{

		}

		private void textBox2_TextChanged(object sender, EventArgs e)
		{

		}

        private void gridLayout_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
