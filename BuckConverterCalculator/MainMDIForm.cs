// MainMDIForm.cs
using BuckConverterCalculator.Analysis;
using BuckConverterCalculator.Simulation;
using BuckConverterCalculator.BOM;
using BuckConverterCalculator.Database;
using BuckConverterCalculator.PCB;
using BuckConverterCalculator.UI.Dialogs;
using BuckConverterCalculator.UI.Controls;
using BuckConverterCalculator.SchematicEditor;
using BuckConverterCalculator.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BuckConverterCalculator
{
    public partial class MainMDIForm : Form
    {
        // Instancias compartidas para los formularios que requieren parámetros
        private ComponentDatabase componentDatabase;
        private DesignParameters designParameters;
        private CalculationResults calculationResults;

        public MainMDIForm()
        {
            InitializeComponent();
            InicializarDatosCompartidos();
            ConfigurarMDI();
            CrearBarraHerramientas();
            CrearBarraEstado();
        }

        private void InicializarDatosCompartidos()
        {
            // Inicializar base de datos de componentes
            componentDatabase = new ComponentDatabase();

            // Inicializar parámetros de diseño con valores por defecto
            designParameters = new DesignParameters
            {
                InputVoltageMax = 12.0,
                OutputVoltage = 5.0,
                OutputCurrent = 3.0,
                SwitchingFrequency = 100000
            };

            // Inicializar resultados de cálculo vacíos
            calculationResults = new CalculationResults();
        }

        private void ConfigurarMDI()
        {
            this.IsMdiContainer = true;
            this.Text = "Sistema de Ingeniería Eléctrica";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.DarkGray;
        }

        private void CrearBarraHerramientas()
        {
            // Botón Reactores VFD
            ToolStripButton btnReactoresVFD = new ToolStripButton
            {
                Text = "VFD",
                ToolTipText = "Calcular Reactores VFD",
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
            };
            btnReactoresVFD.Click += AbrirCalcularReactoresVFD;

            // Botón Protección Motores
            ToolStripButton btnProteccionMotores = new ToolStripButton
            {
                Text = "Protección",
                ToolTipText = "Protección de Motores",
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
            };
            btnProteccionMotores.Click += AbrirCalculoProteccionMotores;

            // Botón Convertidor Buck
            ToolStripButton btnBuck = new ToolStripButton
            {
                Text = "Buck",
                ToolTipText = "Convertidor Buck",
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
            };
            btnBuck.Click += AbrirCalcularBuck;

            // Botón Editor Esquemático
            ToolStripButton btnEditor = new ToolStripButton
            {
                Text = "Editor",
                ToolTipText = "Editor Esquemático",
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
            };
            btnEditor.Click += AbrirSchematicEditor;

            // Botón PCB
            ToolStripButton btnPCB = new ToolStripButton
            {
                Text = "PCB",
                ToolTipText = "Diseño de PCB",
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
            };
            btnPCB.Click += AbrirPCBLayout;

            // Separador
            ToolStripSeparator separador1 = new ToolStripSeparator();

            // Botón Cascada
            ToolStripButton btnCascada = new ToolStripButton
            {
                Text = "Cascada",
                ToolTipText = "Organizar ventanas en cascada",
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
            };
            btnCascada.Click += (s, e) => this.LayoutMdi(MdiLayout.Cascade);

            // Botón Cerrar Todo
            ToolStripButton btnCerrarTodo = new ToolStripButton
            {
                Text = "Cerrar Todo",
                ToolTipText = "Cerrar todas las ventanas",
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
            };
            btnCerrarTodo.Click += CerrarTodasVentanas;

            // Agregar botones a la barra
            barraHerramientas.Items.Add(btnReactoresVFD);
            barraHerramientas.Items.Add(btnProteccionMotores);
            barraHerramientas.Items.Add(btnBuck);
            barraHerramientas.Items.Add(btnEditor);
            barraHerramientas.Items.Add(btnPCB);
            barraHerramientas.Items.Add(separador1);
            barraHerramientas.Items.Add(btnCascada);
            barraHerramientas.Items.Add(btnCerrarTodo);
        }

        private void CrearBarraEstado()
        {
            // Etiqueta de estado
            lblEstado.Text = "Listo";
            lblEstado.Spring = true;
            lblEstado.TextAlign = ContentAlignment.MiddleLeft;

            // Etiqueta de ventanas abiertas
            lblVentanasAbiertas.Text = "Ventanas: 0";
            lblVentanasAbiertas.AutoSize = true;

            // Etiqueta de fecha/hora
            lblFechaHora.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            lblFechaHora.AutoSize = true;

            barraEstado.Items.Add(lblEstado);
            barraEstado.Items.Add(lblVentanasAbiertas);
            barraEstado.Items.Add(lblFechaHora);

            // Timer para actualizar hora
            System.Windows.Forms.Timer timerHora = new System.Windows.Forms.Timer { Interval = 1000 };
            timerHora.Tick += (s, e) => lblFechaHora.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            timerHora.Start();
        }

        private void ActualizarBarraEstado(string mensaje)
        {
            lblEstado.Text = mensaje;
            lblVentanasAbiertas.Text = $"Ventanas: {this.MdiChildren.Length}";
        }

        // Métodos para abrir formularios - Motores y VFD
        private void AbrirCalcularReactoresVFD(object sender, EventArgs e)
        {
            AbrirFormularioHijo<CalcularReactoresVFD>("Reactores VFD");
        }

        private void AbrirCalculoProteccionMotores(object sender, EventArgs e)
        {
            AbrirFormularioHijo<CalculoProteccionMotores>("Protección de Motores");
        }

        // Transformadores
        private void AbrirTransformerForm(object sender, EventArgs e)
        {
            AbrirFormularioHijo<TransformerForm>("Transformadores");
        }

        // Esquemáticos
        private void AbrirSchematicEditor(object sender, EventArgs e)
        {
            AbrirFormularioConParametros(
                () => new SchematicEditorForm(designParameters, calculationResults),
                typeof(SchematicEditorForm),
                "Editor Esquemático"
            );
        }

        private void AbrirSchematicUnifilar(object sender, EventArgs e)
        {
            AbrirFormularioHijo<SchematicUnifilar>("Esquemático Unifilar");
        }

        private void AbrirSchematicViewer(object sender, EventArgs e)
        {
            AbrirFormularioConParametros(
                () => new SchematicViewerForm(designParameters, calculationResults),
                typeof(SchematicViewerForm),
                "Visualizador Esquemático"
            );
        }

        // Electrónica de Potencia
        private void AbrirCalcularBuck(object sender, EventArgs e)
        {
            AbrirFormularioHijo<CalcularBuck>("Convertidor Buck");
        }

        private void AbrirDCMAnalysis(object sender, EventArgs e)
        {
            AbrirFormularioHijo<DCMAnalysisDialog>("Análisis DCM");
        }

        // Análisis y Simulación
        private void AbrirBodePlot(object sender, EventArgs e)
        {
            AbrirFormularioHijo<BodePlotDialog>("Diagrama de Bode");
        }

        private void AbrirWaveformSimulation(object sender, EventArgs e)
        {
            AbrirFormularioHijo<WaveformSimulationDialog>("Simulación de Formas de Onda");
        }

        // PCB y Componentes
        private void AbrirPCBLayout(object sender, EventArgs e)
        {
            AbrirFormularioHijo<PCBLayoutDialog>("Diseño PCB");
        }

        private void AbrirComponentDatabase(object sender, EventArgs e)
        {
            AbrirFormularioConParametros(
                () => new ComponentDatabaseDialog(componentDatabase),
                typeof(ComponentDatabaseDialog),
                "Base de Datos de Componentes"
            );
        }

        private void AbrirComponentSearch(object sender, EventArgs e)
        {
            AbrirFormularioConParametros(
                () => new ComponentSearchDialog(componentDatabase),
                typeof(ComponentSearchDialog),
                "Buscar Componentes"
            );
        }

        // Método genérico para formularios con constructor sin parámetros
        private void AbrirFormularioHijo<T>(string nombreFormulario) where T : Form, new()
        {
            try
            {
                // Verificar si ya existe una instancia del formulario
                foreach (Form formHijo in this.MdiChildren)
                {
                    if (formHijo is T)
                    {
                        formHijo.Activate();
                        ActualizarBarraEstado($"{nombreFormulario} activado");
                        return;
                    }
                }

                // Crear nueva instancia
                T nuevoFormulario = new T();
                nuevoFormulario.MdiParent = this;
                nuevoFormulario.FormClosed += (s, e) => ActualizarBarraEstado("Listo");
                nuevoFormulario.Show();
                ActualizarBarraEstado($"{nombreFormulario} abierto");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir {nombreFormulario}: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ActualizarBarraEstado("Error al abrir formulario");
            }
        }

        // Método para formularios con constructores parametrizados
        private void AbrirFormularioConParametros(Func<Form> crearFormulario, Type tipoFormulario, string nombreFormulario)
        {
            try
            {
                // Verificar si ya existe una instancia del formulario por tipo
                foreach (Form formHijo in this.MdiChildren)
                {
                    if (formHijo.GetType() == tipoFormulario)
                    {
                        formHijo.Activate();
                        ActualizarBarraEstado($"{nombreFormulario} activado");
                        return;
                    }
                }

                // Crear nueva instancia
                Form nuevoFormulario = crearFormulario();
                nuevoFormulario.MdiParent = this;
                nuevoFormulario.FormClosed += (s, e) => ActualizarBarraEstado("Listo");
                nuevoFormulario.Show();
                ActualizarBarraEstado($"{nombreFormulario} abierto");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir {nombreFormulario}: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ActualizarBarraEstado("Error al abrir formulario");
            }
        }

        // Gestión de ventanas
        private void CerrarTodasVentanas(object sender, EventArgs e)
        {
            foreach (Form formHijo in this.MdiChildren)
            {
                formHijo.Close();
            }
            ActualizarBarraEstado("Todas las ventanas cerradas");
        }

        private void MostrarAcercaDe(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Sistema de Ingeniería Eléctrica y Electrónica\n\n" +
                "Versión 1.0\n\n" +
                "Herramientas integradas para:\n" +
                "• Cálculos de motores y VFD\n" +
                "• Diseño de transformadores\n" +
                "• Esquemáticos eléctricos\n" +
                "• Convertidores de potencia\n" +
                "• Análisis y simulación\n" +
                "• Diseño de PCB",
                "Acerca de",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
    }
}