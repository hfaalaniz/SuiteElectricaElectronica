// MainMDIForm.Designer.cs
namespace BuckConverterCalculator
{
    partial class MainMDIForm
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuPrincipal = new System.Windows.Forms.MenuStrip();
            this.menuMotores = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemCalcularReactoresVFD = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemProteccionMotores = new System.Windows.Forms.ToolStripMenuItem();
            this.menuTransformadores = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemTransformerForm = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEsquematicos = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSchematicEditor = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSchematicUnifilar = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSchematicViewer = new System.Windows.Forms.ToolStripMenuItem();
            this.menuElectronica = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemCalcularBuck = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemDCMAnalysis = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAnalisis = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemBodePlot = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemWaveformSimulation = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPCB = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemPCBLayout = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemComponentDatabase = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemComponentSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.menuVentanas = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemCascada = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemMosaicoHorizontal = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemMosaicoVertical = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemCerrarTodo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAyuda = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemAcercaDe = new System.Windows.Forms.ToolStripMenuItem();
            this.barraHerramientas = new System.Windows.Forms.ToolStrip();
            this.barraEstado = new System.Windows.Forms.StatusStrip();
            this.lblEstado = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblVentanasAbiertas = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblFechaHora = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuPrincipal.SuspendLayout();
            this.barraEstado.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuPrincipal
            // 
            this.menuPrincipal.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuPrincipal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuMotores,
            this.menuTransformadores,
            this.menuEsquematicos,
            this.menuElectronica,
            this.menuAnalisis,
            this.menuPCB,
            this.menuVentanas,
            this.menuAyuda});
            this.menuPrincipal.Location = new System.Drawing.Point(0, 0);
            this.menuPrincipal.Name = "menuPrincipal";
            this.menuPrincipal.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.menuPrincipal.Size = new System.Drawing.Size(1200, 28);
            this.menuPrincipal.TabIndex = 0;
            this.menuPrincipal.Text = "menuStrip1";
            // 
            // menuMotores
            // 
            this.menuMotores.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemCalcularReactoresVFD,
            this.menuItemProteccionMotores});
            this.menuMotores.Name = "menuMotores";
            this.menuMotores.Size = new System.Drawing.Size(123, 24);
            this.menuMotores.Text = "Motores y VFD";
            // 
            // menuItemCalcularReactoresVFD
            // 
            this.menuItemCalcularReactoresVFD.Name = "menuItemCalcularReactoresVFD";
            this.menuItemCalcularReactoresVFD.Size = new System.Drawing.Size(241, 26);
            this.menuItemCalcularReactoresVFD.Text = "Calcular Reactores VFD";
            this.menuItemCalcularReactoresVFD.Click += new System.EventHandler(this.AbrirCalcularReactoresVFD);
            // 
            // menuItemProteccionMotores
            // 
            this.menuItemProteccionMotores.Name = "menuItemProteccionMotores";
            this.menuItemProteccionMotores.Size = new System.Drawing.Size(241, 26);
            this.menuItemProteccionMotores.Text = "Protección de Motores";
            this.menuItemProteccionMotores.Click += new System.EventHandler(this.AbrirCalculoProteccionMotores);
            // 
            // menuTransformadores
            // 
            this.menuTransformadores.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemTransformerForm});
            this.menuTransformadores.Name = "menuTransformadores";
            this.menuTransformadores.Size = new System.Drawing.Size(138, 24);
            this.menuTransformadores.Text = "Transformadores";
            // 
            // menuItemTransformerForm
            // 
            this.menuItemTransformerForm.Name = "menuItemTransformerForm";
            this.menuItemTransformerForm.Size = new System.Drawing.Size(275, 26);
            this.menuItemTransformerForm.Text = "Cálculo de Transformadores";
            this.menuItemTransformerForm.Click += new System.EventHandler(this.AbrirTransformerForm);
            // 
            // menuEsquematicos
            // 
            this.menuEsquematicos.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemSchematicEditor,
            this.menuItemSchematicUnifilar,
            this.menuItemSchematicViewer});
            this.menuEsquematicos.Name = "menuEsquematicos";
            this.menuEsquematicos.Size = new System.Drawing.Size(116, 24);
            this.menuEsquematicos.Text = "Esquemáticos";
            // 
            // menuItemSchematicEditor
            // 
            this.menuItemSchematicEditor.Name = "menuItemSchematicEditor";
            this.menuItemSchematicEditor.Size = new System.Drawing.Size(246, 26);
            this.menuItemSchematicEditor.Text = "Editor Esquemático";
            this.menuItemSchematicEditor.Click += new System.EventHandler(this.AbrirSchematicEditor);
            // 
            // menuItemSchematicUnifilar
            // 
            this.menuItemSchematicUnifilar.Name = "menuItemSchematicUnifilar";
            this.menuItemSchematicUnifilar.Size = new System.Drawing.Size(246, 26);
            this.menuItemSchematicUnifilar.Text = "Esquemático Unifilar";
            this.menuItemSchematicUnifilar.Click += new System.EventHandler(this.AbrirSchematicUnifilar);
            // 
            // menuItemSchematicViewer
            // 
            this.menuItemSchematicViewer.Name = "menuItemSchematicViewer";
            this.menuItemSchematicViewer.Size = new System.Drawing.Size(246, 26);
            this.menuItemSchematicViewer.Text = "Visualizador Esquemático";
            this.menuItemSchematicViewer.Click += new System.EventHandler(this.AbrirSchematicViewer);
            // 
            // menuElectronica
            // 
            this.menuElectronica.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemCalcularBuck,
            this.menuItemDCMAnalysis});
            this.menuElectronica.Name = "menuElectronica";
            this.menuElectronica.Size = new System.Drawing.Size(186, 24);
            this.menuElectronica.Text = "Electrónica de Potencia";
            // 
            // menuItemCalcularBuck
            // 
            this.menuItemCalcularBuck.Name = "menuItemCalcularBuck";
            this.menuItemCalcularBuck.Size = new System.Drawing.Size(202, 26);
            this.menuItemCalcularBuck.Text = "Convertidor Buck";
            this.menuItemCalcularBuck.Click += new System.EventHandler(this.AbrirCalcularBuck);
            // 
            // menuItemDCMAnalysis
            // 
            this.menuItemDCMAnalysis.Name = "menuItemDCMAnalysis";
            this.menuItemDCMAnalysis.Size = new System.Drawing.Size(202, 26);
            this.menuItemDCMAnalysis.Text = "Análisis DCM";
            this.menuItemDCMAnalysis.Click += new System.EventHandler(this.AbrirDCMAnalysis);
            // 
            // menuAnalisis
            // 
            this.menuAnalisis.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemBodePlot,
            this.menuItemWaveformSimulation});
            this.menuAnalisis.Name = "menuAnalisis";
            this.menuAnalisis.Size = new System.Drawing.Size(166, 24);
            this.menuAnalisis.Text = "Análisis y Simulación";
            // 
            // menuItemBodePlot
            // 
            this.menuItemBodePlot.Name = "menuItemBodePlot";
            this.menuItemBodePlot.Size = new System.Drawing.Size(289, 26);
            this.menuItemBodePlot.Text = "Diagrama de Bode";
            this.menuItemBodePlot.Click += new System.EventHandler(this.AbrirBodePlot);
            // 
            // menuItemWaveformSimulation
            // 
            this.menuItemWaveformSimulation.Name = "menuItemWaveformSimulation";
            this.menuItemWaveformSimulation.Size = new System.Drawing.Size(289, 26);
            this.menuItemWaveformSimulation.Text = "Simulación de Formas de Onda";
            this.menuItemWaveformSimulation.Click += new System.EventHandler(this.AbrirWaveformSimulation);
            // 
            // menuPCB
            // 
            this.menuPCB.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemPCBLayout,
            this.menuItemComponentDatabase,
            this.menuItemComponentSearch});
            this.menuPCB.Name = "menuPCB";
            this.menuPCB.Size = new System.Drawing.Size(162, 24);
            this.menuPCB.Text = "PCB y Componentes";
            // 
            // menuItemPCBLayout
            // 
            this.menuItemPCBLayout.Name = "menuItemPCBLayout";
            this.menuItemPCBLayout.Size = new System.Drawing.Size(295, 26);
            this.menuItemPCBLayout.Text = "Diseño PCB";
            this.menuItemPCBLayout.Click += new System.EventHandler(this.AbrirPCBLayout);
            // 
            // menuItemComponentDatabase
            // 
            this.menuItemComponentDatabase.Name = "menuItemComponentDatabase";
            this.menuItemComponentDatabase.Size = new System.Drawing.Size(295, 26);
            this.menuItemComponentDatabase.Text = "Base de Datos de Componentes";
            this.menuItemComponentDatabase.Click += new System.EventHandler(this.AbrirComponentDatabase);
            // 
            // menuItemComponentSearch
            // 
            this.menuItemComponentSearch.Name = "menuItemComponentSearch";
            this.menuItemComponentSearch.Size = new System.Drawing.Size(295, 26);
            this.menuItemComponentSearch.Text = "Buscar Componentes";
            this.menuItemComponentSearch.Click += new System.EventHandler(this.AbrirComponentSearch);
            // 
            // menuVentanas
            // 
            this.menuVentanas.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemCascada,
            this.menuItemMosaicoHorizontal,
            this.menuItemMosaicoVertical,
            this.toolStripSeparator1,
            this.menuItemCerrarTodo});
            this.menuVentanas.Name = "menuVentanas";
            this.menuVentanas.Size = new System.Drawing.Size(82, 24);
            this.menuVentanas.Text = "Ventanas";
            // 
            // menuItemCascada
            // 
            this.menuItemCascada.Name = "menuItemCascada";
            this.menuItemCascada.Size = new System.Drawing.Size(215, 26);
            this.menuItemCascada.Text = "Cascada";
            this.menuItemCascada.Click += new System.EventHandler(this.MenuItemCascada_Click);
            // 
            // menuItemMosaicoHorizontal
            // 
            this.menuItemMosaicoHorizontal.Name = "menuItemMosaicoHorizontal";
            this.menuItemMosaicoHorizontal.Size = new System.Drawing.Size(215, 26);
            this.menuItemMosaicoHorizontal.Text = "Mosaico Horizontal";
            this.menuItemMosaicoHorizontal.Click += new System.EventHandler(this.MenuItemMosaicoHorizontal_Click);
            // 
            // menuItemMosaicoVertical
            // 
            this.menuItemMosaicoVertical.Name = "menuItemMosaicoVertical";
            this.menuItemMosaicoVertical.Size = new System.Drawing.Size(215, 26);
            this.menuItemMosaicoVertical.Text = "Mosaico Vertical";
            this.menuItemMosaicoVertical.Click += new System.EventHandler(this.MenuItemMosaicoVertical_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(212, 6);
            // 
            // menuItemCerrarTodo
            // 
            this.menuItemCerrarTodo.Name = "menuItemCerrarTodo";
            this.menuItemCerrarTodo.Size = new System.Drawing.Size(215, 26);
            this.menuItemCerrarTodo.Text = "Cerrar Todo";
            this.menuItemCerrarTodo.Click += new System.EventHandler(this.CerrarTodasVentanas);
            // 
            // menuAyuda
            // 
            this.menuAyuda.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemAcercaDe});
            this.menuAyuda.Name = "menuAyuda";
            this.menuAyuda.Size = new System.Drawing.Size(65, 24);
            this.menuAyuda.Text = "Ayuda";
            // 
            // menuItemAcercaDe
            // 
            this.menuItemAcercaDe.Name = "menuItemAcercaDe";
            this.menuItemAcercaDe.Size = new System.Drawing.Size(169, 26);
            this.menuItemAcercaDe.Text = "Acerca de...";
            this.menuItemAcercaDe.Click += new System.EventHandler(this.MostrarAcercaDe);
            // 
            // barraHerramientas
            // 
            this.barraHerramientas.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.barraHerramientas.Location = new System.Drawing.Point(0, 28);
            this.barraHerramientas.Name = "barraHerramientas";
            this.barraHerramientas.Size = new System.Drawing.Size(1200, 25);
            this.barraHerramientas.TabIndex = 1;
            this.barraHerramientas.Text = "toolStrip1";
            // 
            // barraEstado
            // 
            this.barraEstado.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.barraEstado.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblEstado,
            this.lblVentanasAbiertas,
            this.lblFechaHora});
            this.barraEstado.Location = new System.Drawing.Point(0, 778);
            this.barraEstado.Name = "barraEstado";
            this.barraEstado.Padding = new System.Windows.Forms.Padding(1, 0, 18, 0);
            this.barraEstado.Size = new System.Drawing.Size(1200, 22);
            this.barraEstado.TabIndex = 2;
            this.barraEstado.Text = "statusStrip1";
            // 
            // lblEstado
            // 
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(39, 17);
            this.lblEstado.Text = "Listo";
            // 
            // lblVentanasAbiertas
            // 
            this.lblVentanasAbiertas.Name = "lblVentanasAbiertas";
            this.lblVentanasAbiertas.Size = new System.Drawing.Size(72, 17);
            this.lblVentanasAbiertas.Text = "Ventanas: 0";
            // 
            // lblFechaHora
            // 
            this.lblFechaHora.Name = "lblFechaHora";
            this.lblFechaHora.Size = new System.Drawing.Size(103, 17);
            this.lblFechaHora.Text = "01/01/2025 00:00";
            // 
            // MainMDIForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 800);
            this.Controls.Add(this.barraEstado);
            this.Controls.Add(this.barraHerramientas);
            this.Controls.Add(this.menuPrincipal);
            this.MainMenuStrip = this.menuPrincipal;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainMDIForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sistema de Ingeniería Eléctrica";
            this.menuPrincipal.ResumeLayout(false);
            this.menuPrincipal.PerformLayout();
            this.barraEstado.ResumeLayout(false);
            this.barraEstado.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        // Manejadores de eventos para los menús de ventanas
        private void MenuItemCascada_Click(object sender, System.EventArgs e)
        {
            this.LayoutMdi(System.Windows.Forms.MdiLayout.Cascade);
        }

        private void MenuItemMosaicoHorizontal_Click(object sender, System.EventArgs e)
        {
            this.LayoutMdi(System.Windows.Forms.MdiLayout.TileHorizontal);
        }

        private void MenuItemMosaicoVertical_Click(object sender, System.EventArgs e)
        {
            this.LayoutMdi(System.Windows.Forms.MdiLayout.TileVertical);
        }

        #endregion

        private System.Windows.Forms.MenuStrip menuPrincipal;
        private System.Windows.Forms.ToolStripMenuItem menuMotores;
        private System.Windows.Forms.ToolStripMenuItem menuItemCalcularReactoresVFD;
        private System.Windows.Forms.ToolStripMenuItem menuItemProteccionMotores;
        private System.Windows.Forms.ToolStripMenuItem menuTransformadores;
        private System.Windows.Forms.ToolStripMenuItem menuItemTransformerForm;
        private System.Windows.Forms.ToolStripMenuItem menuEsquematicos;
        private System.Windows.Forms.ToolStripMenuItem menuItemSchematicEditor;
        private System.Windows.Forms.ToolStripMenuItem menuItemSchematicUnifilar;
        private System.Windows.Forms.ToolStripMenuItem menuItemSchematicViewer;
        private System.Windows.Forms.ToolStripMenuItem menuElectronica;
        private System.Windows.Forms.ToolStripMenuItem menuItemCalcularBuck;
        private System.Windows.Forms.ToolStripMenuItem menuItemDCMAnalysis;
        private System.Windows.Forms.ToolStripMenuItem menuAnalisis;
        private System.Windows.Forms.ToolStripMenuItem menuItemBodePlot;
        private System.Windows.Forms.ToolStripMenuItem menuItemWaveformSimulation;
        private System.Windows.Forms.ToolStripMenuItem menuPCB;
        private System.Windows.Forms.ToolStripMenuItem menuItemPCBLayout;
        private System.Windows.Forms.ToolStripMenuItem menuItemComponentDatabase;
        private System.Windows.Forms.ToolStripMenuItem menuItemComponentSearch;
        private System.Windows.Forms.ToolStripMenuItem menuVentanas;
        private System.Windows.Forms.ToolStripMenuItem menuItemCascada;
        private System.Windows.Forms.ToolStripMenuItem menuItemMosaicoHorizontal;
        private System.Windows.Forms.ToolStripMenuItem menuItemMosaicoVertical;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuItemCerrarTodo;
        private System.Windows.Forms.ToolStripMenuItem menuAyuda;
        private System.Windows.Forms.ToolStripMenuItem menuItemAcercaDe;
        private System.Windows.Forms.ToolStrip barraHerramientas;
        private System.Windows.Forms.StatusStrip barraEstado;
        private System.Windows.Forms.ToolStripStatusLabel lblEstado;
        private System.Windows.Forms.ToolStripStatusLabel lblVentanasAbiertas;
        private System.Windows.Forms.ToolStripStatusLabel lblFechaHora;
    }
}