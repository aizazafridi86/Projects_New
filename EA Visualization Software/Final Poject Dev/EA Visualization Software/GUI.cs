﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using EA_Visualization_Software.PSO_Algorithm;
using EA_Visualization_Software.Common;
using System.Text.RegularExpressions;
using EA_Visualization_Software.GA;
using Microsoft.Office.Core;
//using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;

namespace EA_Visualization_Software
{
    public partial class EAVisualizationPanel : Form
    {
        //public Output output;
        public List<Output> output;
        public int numberOfTimeSteps;
        public int numberOfRuns;

        public List<string> uniqueGenesFound = new List<string>();
        public int GANumberOfGenes;
        public CountGenes count_genes = new CountGenes();

        public string GAInputFunction;
        public int crossover_points;
        public bool validate_populationsize;

        public string PSOInputFunction;
        public int PSONumberOfGenes;

        //dictionaries that stores average of bestFitness,averageFitness and dispersion of all runs for each timestep
        Dictionary<int, double> bestFitness_Average = new Dictionary<int, double>();
        Dictionary<int, double> averageFitness_Average = new Dictionary<int, double>();
        Dictionary<int, double> dispersion_Average = new Dictionary<int, double>();

        public string saving_directory;

        TestIInputFunction test_input_function = new TestIInputFunction();
        public EAVisualizationPanel()
        {
            InitializeComponent();
            this.showLaunchPanel();
        }

        #region ParticleSwarmData
        //Launching PS Panel for the user to enter values for Particle Swarm optimization Algorithm
        private void LaunchPS_Click(object sender, EventArgs e)
        {
            //showing PS Panel
            this.showPSPanel();
            //disable output options 
            this.DisableOutPutOptions();
            //hiding error messages labels
            this.HideErrorMessages();
            //clearing output panel
            this.clear_output_panel();
        }

        //Launching Particle Swarm Optimization Algorithm
        private void runPSOButton_Click(object sender, EventArgs e)
        {
            //hiding error messages
            this.HideErrorMessages();

            //validating all values
            bool validation = this.Validate_PS_Values();

            //run the algorithm only if validation is successfull
            if(validation==true)
            {
                //getting all user input values and storing them in local variables
                string inputFunction = this.inputFunctionTextBox.Text.ToString();
                double lowerRange = Convert.ToDouble(this.lowerRangeTextBox.Text);
                double higherRange = Convert.ToDouble(this.higherRangeTextBox.Text);
                string updateRule = this.updateRuleComboBox.Items[this.updateRuleComboBox.SelectedIndex].ToString();
                int posClampVelocity = Convert.ToInt32(this.posClampVelocityTextBox.Text);
                int negClampVelocity = Convert.ToInt32(this.negClampVelocityTextBox.Text);
                int numberOfParticles = Convert.ToInt32(this.numberOfParticlesComboBox.Items[this.numberOfParticlesComboBox.SelectedIndex]);
                int numberOfTimeSteps = Convert.ToInt32(this.numberOfTimeStepsComboBox.Items[this.numberOfTimeStepsComboBox.SelectedIndex]);
                int numberOfRuns = Convert.ToInt32(this.numberOfRunsComboBox.Items[this.numberOfRunsComboBox.SelectedIndex]);
                string optimizationType;

                this.numberOfTimeSteps = numberOfTimeSteps;
                this.numberOfRuns = numberOfRuns;

                if (this.minimizeRadioButton.Checked)
                {
                    optimizationType = this.minimizeRadioButton.Text.ToString();
                }

                else
                {
                    optimizationType = this.maximizeRadioButton.Text.ToString();
                }

                //Creating Instance of PS Controller
                PSController controller = new PSController();

                //passing variable values to Controller
                controller.Initialize(inputFunction, lowerRange, higherRange, updateRule, posClampVelocity, negClampVelocity, numberOfParticles, numberOfTimeSteps, numberOfRuns, optimizationType);

                //Running PSO Algorithm
                this.output = controller.RunPSAlgorithm();

                //enabling output controls
                this.enable_output_panel_controls();

                //calculating average of each timeStep of all runs
                this.CalculateAverage();

                //appending data to text boxes
                this.ShowDataInTextBoxes();

                //method that plot charts
                this.PlotCharts();

                //disable all controls when the output is generated
                this.DisableAll();

                //enable output controls
                this.EnableOutputOptions();
            }

        }

        //method that calcualtes the average of bestFitness,averageFitness and dispersion in each time step for all runs
        public void CalculateAverage()
        {

            //calculating average of best fitness, average fitness and dispersion, storing the values in dictionaries and showing it in 
            //text box in GUI
            for (int timeStep = 1; timeStep <= this.numberOfTimeSteps; timeStep++)
            {
                //variables for calculating average
                double best = 0;
                double average = 0;
                double disp = 0;

                //calculating average
                for (int run = 0; run < this.numberOfRuns; run++)
                {
                    best = best + this.output[run].bestFitness[timeStep];
                    average = average + this.output[run].averageFitness[timeStep];
                    disp = disp + this.output[run].dispersion[timeStep];
                }
                best = best / this.numberOfRuns;
                average = average / this.numberOfRuns;
                disp = disp / this.numberOfRuns;

                //adding values to dictionaries
                this.bestFitness_Average.Add(timeStep, best);
                this.averageFitness_Average.Add(timeStep, average);
                this.dispersion_Average.Add(timeStep, disp);
            }
        }

       //method that appends data to the textboxes
       public void ShowDataInTextBoxes()
       {
            
           for(int timeStep=1; timeStep<=this.numberOfTimeSteps; timeStep++)
           {
              OutputbestFitness.AppendText(timeStep.ToString() + ".\t");
              OutputbestFitness.AppendText(this.bestFitness_Average[timeStep].ToString());
              OutputbestFitness.AppendText("\n");

              outputAverageFitness.AppendText(timeStep.ToString() + ".\t");
              outputAverageFitness.AppendText(this.averageFitness_Average[timeStep].ToString());
              outputAverageFitness.AppendText("\n");

              outputDispersion.AppendText(timeStep.ToString() + ".\t");
              outputDispersion.AppendText(this.dispersion_Average[timeStep].ToString());
              outputDispersion.AppendText("\n");
           }

        }

        public void PlotCharts()
       {
           //plotting best individual fitness in each time step
           //chart1.Series.Add("Best Fitness");
           for (int timeStep = 1; timeStep <= this.numberOfTimeSteps; timeStep++)
           {
               chart1.Series["Best Fitness"].Points.AddXY(Convert.ToDouble(timeStep), this.bestFitness_Average[timeStep].ToString());
           }

           //plotting population average fitness in each time step
           //chart2.Series.Add("Average Fitness");
           for (int timeStep = 1; timeStep <= this.numberOfTimeSteps; timeStep++)
           {       
               GAHigherRangeTextBox.Series["Average Fitness"].Points.AddXY(Convert.ToDouble(timeStep), this.averageFitness_Average[timeStep].ToString());
           }

           //plotting population dispersion in each time step
           //chart3.Series.Add("Dispersion");
           for (int timeStep = 1; timeStep <= this.numberOfTimeSteps; timeStep++)
           {  
               chart3.Series["Dispersion"].Points.AddXY(Convert.ToDouble(timeStep), this.dispersion_Average[timeStep].ToString());
           }

       }

        //method that shows best fitness plot
        private void bestFitnessPlot_Click(object sender, EventArgs e)
        {
            this.plotPanel.Show();
            chart1.Show();
            GAHigherRangeTextBox.Hide();
            chart3.Hide();
        }

        //method that shows population average fitness plot
        private void averageFitnessPlot_Click(object sender, EventArgs e)
        {
            this.plotPanel.Show();
            GAHigherRangeTextBox.Show();
            chart1.Hide();
            chart3.Hide();         
        }

        //method that shows population dispersion plot
        private void dispersionPlot_Click(object sender, EventArgs e)
        {
            this.plotPanel.Show();
            chart3.Show();
            chart1.Hide();
            GAHigherRangeTextBox.Hide();                          
        }

        //method that shows the output data
        private void showDataButton_Click(object sender, EventArgs e)
        {
            chart1.Hide();
            GAHigherRangeTextBox.Hide();
            chart3.Hide();
            this.showPSPanel();
        }

        //managing panels
        public void showPSPanel()
        {
            this.PSPanel.Show();
            this.launchPanel.Hide();
            this.outputPanel.Show();
            this.plotPanel.Hide();
            this.GAPanel.Hide();
        }

        public void showLaunchPanel()
        {
            this.launchPanel.Show();
            this.PSPanel.Hide();
            this.outputPanel.Hide();
            this.plotPanel.Hide();
            this.GAPanel.Hide();
           
        }

        public void showPlotPanel()
        {
            
            this.PSPanel.Show();
            this.launchPanel.Hide();
            this.outputPanel.Hide();
            this.GAPanel.Hide();
            this.plotPanel.Show();
        }

        public bool Validate_PS_Values()
        {
            bool validation = true;

            //validating input function
            if (this.inputFunctionTextBox.Text == "")
            {
                this.inputFunctionErrLabel.Visible = true;
                validation = false;
            }
            if (this.inputFunctionTextBox.Text != "")
            {
                if (test_input_function.test_function(this.inputFunctionTextBox.Text.ToString(), this.uniqueGenesFound) == false)
                {
                    this.inputFunctionErrLabel.Visible = true;
                    validation = false;
                }
            }

            //validating lower range
            if (!System.Text.RegularExpressions.Regex.IsMatch(this.lowerRangeTextBox.Text, "[ ^ 0-9]"))
            {
                this.lowerRangeErrLabel.Visible = true;
                validation = false;
            }
            if (this.lowerRangeTextBox.Text != "")
            {
                try
                {
                    Convert.ToDouble(this.lowerRangeTextBox.Text.ToString());
                }
                catch (FormatException)
                {
                    this.lowerRangeErrLabel.Visible = true;
                    validation = false;
                }
            }

            //validating higher range
            if (!System.Text.RegularExpressions.Regex.IsMatch(this.higherRangeTextBox.Text, "[ ^ 0-9]"))
            {
                this.higherRangeErrLabel.Visible = true;
                validation = false;
            }
            if (this.higherRangeTextBox.Text != "")
            {
                try
                {
                    Convert.ToDouble(this.higherRangeTextBox.Text.ToString());
                }
                catch (FormatException)
                {
                    this.higherRangeErrLabel.Visible = true;
                    validation = false;
                }
            }
            

            //validating update rule combo box
            if(this.updateRuleComboBox.SelectedIndex == -1)
            {
                validation = false;
                this.velocityUpdateErrLabel.Show();
            }

            //validating positive clamp velocity
            if (!System.Text.RegularExpressions.Regex.IsMatch(this.posClampVelocityTextBox.Text, "[ ^ 0-9]"))
            {
                this.posVelErrLabel.Visible = true;
                validation = false;
            }
            if (this.posClampVelocityTextBox.Text != "")
            {
                try
                {
                    Convert.ToInt32(this.posClampVelocityTextBox.Text.ToString());
                }
                catch (FormatException)
                {
                    this.posVelErrLabel.Visible = true;
                    validation = false;
                }
            }

            //validating negative clamp velocity
            if (!System.Text.RegularExpressions.Regex.IsMatch(this.negClampVelocityTextBox.Text, "[ ^ 0-9]"))
            {
                this.negVelErrLabel.Visible = true;
                validation = false;
            }
            if (this.negClampVelocityTextBox.Text != "")
            {
                try
                {
                    Convert.ToInt32(this.negClampVelocityTextBox.Text.ToString());
                }
                catch (FormatException)
                {
                    this.negVelErrLabel.Visible = true;
                    validation = false;
                }
            }

            //validating number of particles combox
            if (this.numberOfParticlesComboBox.SelectedIndex == -1)
            {
                validation = false;
                this.particleErrLabel.Show();
            }

            //validating number of time steps combobox
            if (this.numberOfTimeStepsComboBox.SelectedIndex == -1)
            {
                validation = false;
                this.timeStepsErrLabel.Show();
            }

            //validating number of runs combo box
            if (this.numberOfRunsComboBox.SelectedIndex == -1)
            {
                validation = false;
                this.runsErrLabel.Show();
            }
             
            return validation;
            
        }

        //method that resets everything
        private void resetButton_Click(object sender, EventArgs e)
        {
            //enable all controls
            this.EnableAll();

            //showing PS Panel
            this.showPSPanel();

            //clearing charts data
            foreach (var series in chart1.Series)
            {
                series.Points.Clear();
            }
            foreach (var series in GAHigherRangeTextBox.Series)
            {
                series.Points.Clear();
            }
            foreach (var series in chart3.Series)
            {
                series.Points.Clear();
            }
        }

        //method that enables all controls
        public void EnableAll()
        {
            this.inputFunctionTextBox.Enabled = true;
            this.lowerRangeTextBox.Enabled = true;
            this.higherRangeTextBox.Enabled = true;
            this.updateRuleComboBox.Enabled = true;
            this.posClampVelocityTextBox.Enabled = true;
            this.negClampVelocityTextBox.Enabled = true;
            this.numberOfParticlesComboBox.Enabled = true;
            this.numberOfTimeStepsComboBox.Enabled = true;
            this.numberOfRunsComboBox.Enabled = true;
            this.runPSOButton.Enabled = true;

            this.bestFitnessPlot.Enabled = true;
            this.averageFitnessPlot.Enabled = true;
            this.dispersionPlot.Enabled = true;
            this.showDataButton.Enabled = true;

            this.outputAverageFitness.Clear();
            this.OutputbestFitness.Clear();
            this.outputDispersion.Clear();

            this.bestFitnessPlot.Enabled = false;
            this.averageFitnessPlot.Enabled = false;
            this.dispersionPlot.Enabled = false;
            this.showDataButton.Enabled = false;

            //clearing dictionaries
            this.bestFitness_Average.Clear();
            this.averageFitness_Average.Clear();
            this.dispersion_Average.Clear();
        }

        //method that disables all controls
        public void DisableAll()
        {
            this.inputFunctionTextBox.Enabled = false;
            this.lowerRangeTextBox.Enabled = false;
            this.higherRangeTextBox.Enabled = false;
            this.updateRuleComboBox.Enabled = false;
            this.posClampVelocityTextBox.Enabled = false;
            this.negClampVelocityTextBox.Enabled = false;
            this.numberOfParticlesComboBox.Enabled = false;
            this.numberOfTimeStepsComboBox.Enabled = false;
            this.numberOfRunsComboBox.Enabled = false;
            this.runPSOButton.Enabled = false;
            this.runPSOButton.Enabled = false;
        }

        //method that enables output options
        public void EnableOutputOptions()
        {
            this.bestFitnessPlot.Enabled = true;
            this.averageFitnessPlot.Enabled = true;
            this.dispersionPlot.Enabled = true;
            this.showDataButton.Enabled = true;
        }

        //method that disables output options
        public void DisableOutPutOptions()
        {
            this.bestFitnessPlot.Enabled = false;
            this.averageFitnessPlot.Enabled = false;
            this.dispersionPlot.Enabled = false;
            this.showDataButton.Enabled = false;
        }

        //method that hides error messages
        public void HideErrorMessages()
        {
            this.inputFunctionErrLabel.Hide();
            this.lowerRangeErrLabel.Hide();
            this.higherRangeErrLabel.Hide();
            this.velocityUpdateErrLabel.Hide();
            this.posVelErrLabel.Hide();
            this.negVelErrLabel.Hide();
            this.particleErrLabel.Hide();
            this.timeStepsErrLabel.Hide();
            this.runsErrLabel.Hide();
        }

        //method that is called when the user leaves PSO input function textbox
        private void inputFunctionTextBox_Leave(object sender, EventArgs e)
        {
            bool validation = true;

            if (inputFunctionTextBox.Text == "")
            {
                validation = false;
            }

            if (validation == true)
            {

                //counting number of unique genes
                this.PSOInputFunction = this.inputFunctionTextBox.Text.ToString();
                this.uniqueGenesFound = this.count_genes.Return_NumberOfGenes(this.PSOInputFunction);
                this.PSONumberOfGenes = this.uniqueGenesFound.Count;
            }

        }

        #endregion

        #region GAData
        public void launchTournamentSelectionOptions()
        {
            //show tournament selection options
            this.GATournamentSizeLabel.Visible = true;
            this.GATournamentSizeComboBox.Enabled = true;
            this.GATournamentSizeComboBox.Visible = true;

            //get the population size
            int size = Convert.ToInt32(this.GAPopulationSizeTextBox.Text.ToString());

            //add values values of tournament size in combo box
            for (int i = 2; i <= size; i++)
            {
                this.GATournamentSizeComboBox.Items.Add(i);
            }

        }

        //method that activates qSelection textbox 
  

    

        public void hideTournamentSelectionOptions()
        {
            //hide tournament selection options
            this.GATournamentSizeLabel.Visible = false;
            this.GATournamentSizeComboBox.Enabled = false;
            this.GATournamentSizeComboBox.Visible = false;
        }

        private void GASelectionMethodComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //validate population size
            this.validate_populationsize = this.validate_PopSize();

            //launch method to show tournament selection options if tournament selection method is selected and popualtion size is validated
           if(this.GASelectionMethodComboBox.SelectedIndex==0 && this.validate_populationsize==true)
           {
               this.launchTournamentSelectionOptions();
           }
            //if tournement selection is not selected then hide tournament selection options
           else
           {
               this.hideTournamentSelectionOptions();
           }

            //if rank selection is selected then launch text box to get the value of s from user
            if(this.GASelectionMethodComboBox.SelectedIndex==2)
            {
                this.sLabel.Visible = true;
                this.sTextBox.Visible = true;
            }
            else
            {
                this.sLabel.Visible = false;
                this.sTextBox.Visible = false;
            }
        }
        
        //method that is called when the user leaves recombination selection textbox
        private void GARecombinationTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(this.GARecombinationTypeComboBox.SelectedIndex==1 || this.GARecombinationTypeComboBox.SelectedIndex==2 || this.GARecombinationTypeComboBox.SelectedIndex==3)
            {
                this.aLabel.Visible = true;
                this.aTextBox.Visible = true;
            }
            else
            {
                this.aLabel.Visible = false;
                this.aTextBox.Visible = false;
            }
        }

        //method that is called when the index in SurvivorSelectionCombobox changes
        

        //method that launches GA
        private void launchGA_Click(object sender, EventArgs e)
        {
            //show Panel that would get input paramenters from users for GA Algorithm
            this.ShowGAPanel();
            this.clear_output_panel();
        }

        public void ShowGAPanel()
        {
            this.GAPanel.Show();
            this.outputPanel.Show();
            this.launchPanel.Hide();
            this.PSPanel.Hide();
        }

        //method that counts the number of genes when the user inputs input function
        private void GAInputFunctionTextBox_Leave(object sender, EventArgs e)
        {

            //make sure that input Function Textbox contains valid data
            bool validate = this.inputFunction_Valdiation();

            //put the unique genes in uniqeGenesFound list only if validation is true
            if (validate == true)
            {
                //counting number of unique genes
                this.GAInputFunction = this.GAInputFunctionTextBox.Text.ToString();
                this.uniqueGenesFound = this.count_genes.Return_NumberOfGenes(this.GAInputFunction);
                this.GANumberOfGenes = this.uniqueGenesFound.Count;
            }
                
        }

        //method that validates GA Input function
        public bool inputFunction_Valdiation()
        {
            bool validation = true;

            //valdiating input text box
            if(this.GAInputFunctionTextBox.Text == "")
            {
                validation = false;
            }

            return validation;
        }

        //this method makes sure that population size text box contains valid data 
        public bool validate_PopSize()
        {
            bool validation=true;

            //do not validate if the textbox does not contains numbers
            if (!System.Text.RegularExpressions.Regex.IsMatch(this.GAPopulationSizeTextBox.Text, "[ ^ 0-9]"))
            {
                validation = false;
            }

            return validation;

        }

        //this method is launched that validates population size as soon as user leaves the text box
        private void GAPopulationSizeTextBox_TextChanged(object sender, EventArgs e)
        {
            //validate population size
            this.validate_populationsize = this.validate_PopSize();
        }

        //method that runs the GA Algorithm
        private void runGA_Click(object sender, EventArgs e)
        {
            int tournament_size;
            bool validate;

            //hiding error messages
            this.GA_Hide_Error_Messages();

            //validating parameters required to run the algorithm
            validate = this.validate_GA_parameters();

            if (validate == true)
            {
                //getting values from textboxes
                string input_function = this.GAInputFunctionTextBox.Text.ToString();
                double lowerRange = Convert.ToDouble(this.GALowerRangeTextBox.Text.ToString());
                double higherRange = Convert.ToDouble(this.GAHigherTextBox.Text.ToString());
                int pop_size = Convert.ToInt32(this.GAPopulationSizeTextBox.Text.ToString());
                int off_size = Convert.ToInt32(this.offspringSizeTextBox.Text.ToString());
                string selection_method = this.GASelectionMethodComboBox.Items[this.GASelectionMethodComboBox.SelectedIndex].ToString();
                double a;
                double s;
                string optimization_type;

                if (this.GATournamentSizeComboBox.Enabled == true)
                {
                    tournament_size = Convert.ToInt32(this.GATournamentSizeComboBox.Items[this.GATournamentSizeComboBox.SelectedIndex].ToString());
                }
                else
                {
                    tournament_size = 0;
                }

                if (this.aTextBox.Visible == true)
                {
                    a = Convert.ToDouble(this.aTextBox.Text.ToString());
                }
                else
                {
                    a = 0;
                }

                if (this.sTextBox.Visible == true)
                {
                    s = Convert.ToDouble(this.sTextBox.Text.ToString());
                }
                else
                {
                    s = 0;
                }

                string survivor_selection_method = this.GASurvivorSelectionComboBox.Items[this.GASurvivorSelectionComboBox.SelectedIndex].ToString();

                string recombination_type = this.GARecombinationTypeComboBox.Items[this.GARecombinationTypeComboBox.SelectedIndex].ToString();

                double crossover_rate = Convert.ToDouble(this.GACrossoverRateComboBox.Items[this.GACrossoverRateComboBox.SelectedIndex].ToString());
                string mutation_type = this.GAMutationTypeComboBox.Items[this.GAMutationTypeComboBox.SelectedIndex].ToString();
                double mutation_rate = Convert.ToDouble(this.GAMutationRateComboBox.Items[this.GAMutationRateComboBox.SelectedIndex].ToString());

                int number_generations = Convert.ToInt32(GAGenerationsTextBox.Text.ToString());
                this.numberOfTimeSteps = number_generations;
                int number_runs = Convert.ToInt32(GARunsTextBox.Text.ToString());
                this.numberOfRuns = number_runs;

                if (this.GAMaximizeRadioButton.Checked)
                {
                    optimization_type = this.GAMaximizeRadioButton.Text.ToString();
                }
                else
                {
                    optimization_type = this.GAMinimizeRadioButton.Text.ToString();
                }

                //Creating instance of GA Controller
                GAController controller = new GAController();

                //Initializing global values in controller
                controller.Initialize(input_function, lowerRange, higherRange, pop_size, selection_method, tournament_size, recombination_type, crossover_points, crossover_rate, mutation_type, mutation_rate, this.uniqueGenesFound, this.GANumberOfGenes, number_generations, number_runs, survivor_selection_method,off_size, a, s, optimization_type);

                //running GA Algorithm
                this.output = controller.RunGA();

                //enabling output controls
                this.enable_output_panel_controls();

                //calculating average of each timeStep of all runs
                this.CalculateAverage();

                //appending data to text boxes
                this.ShowDataInTextBoxes();

                //method that plot charts
                this.PlotCharts();

                //enable plotting options
                this.GA_Enable_Plot_Options();

                //disabling input parametes
                this.GADisable_All();

                //enabling reset button
                this.GAResetButton.Enabled = true;
            }
        }

        //method that validates GA input parameters
        public bool validate_GA_parameters()
        {
            //setting validate to true
            //validate will be set to false if validation of any required fields fails
            bool validate = true;
            double value;
            double value2;

            //validating input function
            if(this.GAInputFunctionTextBox.Text == "")
            {
                this.GAInputErr.Visible = true;
                validate = false;
            }
            if(this.GAInputFunctionTextBox.Text != "")
            {
                if (test_input_function.test_function(GAInputFunctionTextBox.Text.ToString(), this.uniqueGenesFound) == false)
                {
                    this.GAInputErr.Visible = true;
                    validate = false;
                }
            }
            
            //validating function lower range
            if(!System.Text.RegularExpressions.Regex.IsMatch(this.GALowerRangeTextBox.Text, "[ ^ 0-9]") )
            {
                this.GALowerErr.Visible = true;
                validate = false;
            }
            if(this.GALowerRangeTextBox.Text != "")
            {
                try
                {
                    Convert.ToDouble(this.GALowerRangeTextBox.Text.ToString());
                }
                 catch (FormatException)
                {
                this.GALowerErr.Visible = true;
                validate = false;
                }
            }

            //validating function higher range
            if (!System.Text.RegularExpressions.Regex.IsMatch(this.GAHigherTextBox.Text, "[ ^ 0-9]"))
            {
                this.GAHigherErr.Visible = true;
                validate = false;
            }
             if(this.GAHigherTextBox.Text != "")
            {
                try
                {
                    Convert.ToDouble(this.GAHigherTextBox.Text.ToString());                   
                }
                 catch (FormatException)
                {
                this.GAHigherErr.Visible = true;
                validate = false;
                } 
            }

            //validating individual population size
            if (!System.Text.RegularExpressions.Regex.IsMatch(this.GAPopulationSizeTextBox.Text, "[ ^ 0-9]"))
            {
                this.GAPopulationErr.Visible = true;
                validate = false;
            }
           if(this.GAPopulationSizeTextBox.Text != "")
            {
                try
                {
                    if(Convert.ToDouble(this.GAPopulationSizeTextBox.Text.ToString()) <= 0)
                    {
                        this.GAPopulationErr.Visible = true;
                        validate = false;
                    }     
                }
                 catch (FormatException)
                {
                this.GAPopulationErr.Visible = true;
                validate = false;
                } 
            }

            //validating offspring population size
            if (!System.Text.RegularExpressions.Regex.IsMatch(this.offspringSizeTextBox.Text, "[ ^ 0-9]"))
            {
                this.GAOffErr.Visible = true;
                validate = false;
            }
          if(this.offspringSizeTextBox.Text != "")
            {
                try
                {
                    if(Convert.ToDouble(this.offspringSizeTextBox.Text.ToString()) <= 0)
                    {
                        this.GAOffErr.Visible = true;
                        validate = false;
                    }       
                }
                 catch (FormatException)
                {
                this.GAOffErr.Visible = true;
                validate = false;
                } 
            }

            
            //validating survivor selection method
            if(this.GASelectionMethodComboBox.SelectedIndex == -1)
            {
                this.GAMatingErr.Visible = true;
                validate = false;
            }

            //validating tournament size 
            if(this.GATournamentSizeComboBox.Visible==true)
            {
                if(this.GATournamentSizeComboBox.SelectedIndex == -1)
                {
                    this.GATourErr.Visible = true;
                    validate = false;
                }
            }

            //validating s
            if(this.sTextBox.Visible == true)
            {
                if(!System.Text.RegularExpressions.Regex.IsMatch(this.sTextBox.Text, "[ ^ 0-9]"))
                {
                    this.GASErr.Visible = true;
                    validate = false;
                }
                if(this.sTextBox.Text != "")
                {
                    try
                    {
                        value2 = Convert.ToDouble(this.sTextBox.Text.ToString());
                        if(value2 <= 1 || value2 > 2) 
                        {
                            this.GASErr.Visible = true;
                            validate = false;
                        }

                    }
                    catch (FormatException)
                    {
                        this.GAAErr.Visible = true;
                        validate = false;
                    } 
                }
                
            }

            //validating survivor selection method
            if(this.GASurvivorSelectionComboBox.SelectedIndex == -1)
            {
                this.GASurvivorErr.Visible = true;
                validate = false;
            }


            //validating recombination
            if(this.GARecombinationTypeComboBox.SelectedIndex == -1)
            {
                this.GARecombErr.Visible = true;
                validate = false;
            }

            //validating a
            if(this.aTextBox.Visible == true)
            {
                if(!System.Text.RegularExpressions.Regex.IsMatch(this.aTextBox.Text, "[ ^ 0-9]"))
                {
                    this.GAAErr.Visible = true;
                    validate = false;
                }
            if(this.aTextBox.Text != "")
            {
                try
                {
                    value = Convert.ToDouble(this.aTextBox.Text.ToString());
                    if(value >= 1 || value <= 0) 
                    {
                        this.GAAErr.Visible = true;
                        validate = false;
                    } 
                }
                 catch (FormatException)
                {
                this.GAAErr.Visible = true;
                validate = false;
                } 
            }
           }

            //validating crossover rate
            if(this.GACrossoverRateComboBox.SelectedIndex == -1)
            {
                this.GACrossErr.Visible = true;
                validate = false;
            }

            //validating mutation type
            if(this.GAMutationTypeComboBox.SelectedIndex == -1)
            {
                this.GAMutationTypeErr.Visible = true;
                validate = false;
            }

            //validating mutation rate
            if(this.GAMutationRateComboBox.SelectedIndex == -1)
            {
                this.GAMutationRateErr.Visible = true;
                validate = false;
            }

            //validating number of generations
            if(!System.Text.RegularExpressions.Regex.IsMatch(this.GAGenerationsTextBox.Text, "[ ^ 0-9]"))
            {
                this.GAGenerationsErr.Visible = true;
                validate = false;
            }
            if(this.GAGenerationsTextBox.Text != "")
            {
                try
                {
                    if(Convert.ToDouble(this.GAGenerationsTextBox.Text.ToString()) <= 0)
                    {
                        this.GAGenerationsErr.Visible = true;
                        validate = false;
                    }
                
                    
                }
                 catch (FormatException)
                {
                this.GAGenerationsErr.Visible = true;
                validate = false;
                } 
            }
            

            //validating number of runs
            if (!System.Text.RegularExpressions.Regex.IsMatch(this.GARunsTextBox.Text, "[ ^ 0-9]"))
            {
                this.GARunsErr.Visible = true;
                validate = false;
            }
            if(this.GARunsTextBox.Text != "")
            {
                try
                {
                    if(Convert.ToDouble(this.GARunsTextBox.Text.ToString()) <= 0)
                    {
                        this.GARunsErr.Visible = true;
                        validate = false;
                    }
                
                    
                }
                 catch (FormatException)
                {
                this.GARunsErr.Visible = true;
                validate = false;
                } 
            }
             
            //returning bool validate
            return validate;
             

        }

        //method that disables all GA controls
        public void GADisable_All()
        {
            this.GAInputFunctionTextBox.Enabled = false;
            this.GAMinimizeRadioButton.Enabled = false;
            this.GAMaximizeRadioButton.Enabled = false;
            this.GALowerRangeTextBox.Enabled = false;
            this.GAHigherTextBox.Enabled = false;
            this.GAPopulationSizeTextBox.Enabled = false;
            this.offspringSizeTextBox.Enabled = false;
            this.GASelectionMethodComboBox.Enabled = false;
            this.GATournamentSizeComboBox.Visible = false;
            this.sTextBox.Visible = false;
            this.GASurvivorSelectionComboBox.Enabled = false;
  
            this.GARecombinationTypeComboBox.Enabled = false;
            this.aTextBox.Visible = false;
            this.GACrossoverRateComboBox.Enabled = false;
            this.GAMutationTypeComboBox.Enabled = false;
            this.GAMutationRateComboBox.Enabled = false;
            this.GAGenerationsTextBox.Enabled = false;
            this.GARunsTextBox.Enabled = false;
            this.runGA.Enabled = false;
        }

        //method that enables all GA controls
        public void GAEnable_All()
        {
            this.GAInputFunctionTextBox.Enabled = true;
            this.GAMinimizeRadioButton.Enabled = true;
            this.GAMaximizeRadioButton.Enabled = true;
            this.GALowerRangeTextBox.Enabled = true;
            this.GAHigherTextBox.Enabled = true;
            this.GAPopulationSizeTextBox.Enabled = true;
            this.offspringSizeTextBox.Enabled = true;
            this.GASelectionMethodComboBox.Enabled = true;
            this.GASurvivorSelectionComboBox.Enabled = true;
            this.GARecombinationTypeComboBox.Enabled = true;
            this.GACrossoverRateComboBox.Enabled = true;
            this.GAMutationTypeComboBox.Enabled = true;
            this.GAMutationRateComboBox.Enabled = true;
            this.GAGenerationsTextBox.Enabled = true;
            this.GARunsTextBox.Enabled = true;
        }

        //method that resets all values
        public void GAReset_All()
        {
            //enabling all GA Controls
            this.GAEnable_All();

            //clearing charts
            foreach (var series in chart1.Series)
            {
                series.Points.Clear();
            }
            foreach (var series in GAHigherRangeTextBox.Series)
            {
                series.Points.Clear();
            }
            foreach (var series in chart3.Series)
            {
                series.Points.Clear();
            }

            //clearing dictionaries
            bestFitness_Average.Clear();
            averageFitness_Average.Clear();
            dispersion_Average.Clear();
        }

        //method that hides all GA error messages
        public void GA_Hide_Error_Messages()
        {
            this.GAInputErr.Visible = false;
            this.GALowerErr.Visible = false;
            this.GAHigherErr.Visible = false;
            this.GAPopulationErr.Visible = false;
            this.GAOffErr.Visible = false;
            this.GAMatingErr.Visible = false;
            this.GATourErr.Visible = false;
            this.GASErr.Visible = false;
            this.GASurvivorErr.Visible = false;
            this.GARecombErr.Visible = false;
            this.GAAErr.Visible = false;
            this.GACrossErr.Visible = false;
            this.GAMutationTypeErr.Visible = false;
            this.GAMutationRateErr.Visible = false;
            this.GAGenerationsErr.Visible = false;
            this.GARunsErr.Visible = false;
        }

        //method that shows population best individual fitness plot
        private void GAPlotBestButton_Click(object sender, EventArgs e)
        {
            this.plotPanel.Show();
            chart1.Show();
            GAHigherRangeTextBox.Hide();
            chart3.Hide();
        }

        //method that shows population average fitness plot
        private void GAPlotAveButton_Click(object sender, EventArgs e)
        {
            this.plotPanel.Show();
            GAHigherRangeTextBox.Show();
            chart1.Hide();
            chart3.Hide(); 
        }

        //method that shows population dispersion plot
        private void GAPlotDispersionButton_Click(object sender, EventArgs e)
        {
            this.plotPanel.Show();
            chart3.Show();
            chart1.Hide();
            GAHigherRangeTextBox.Hide();
        }

        //method that enables plotting options
        public void GA_Enable_Plot_Options()
        {
            this.GAPlotAveButton.Enabled = true;
            this.GAPlotBestButton.Enabled = true;
            this.GAPlotDispersionButton.Enabled = true;

            this.GAShowDataButton.Enabled = true;
        }

        //method that disables plotting options
        public void GA_Disable_Plot_Options()
        {
            this.GAPlotAveButton.Enabled = false;
            this.GAPlotBestButton.Enabled = false;
            this.GAPlotDispersionButton.Enabled = false;
        }

        //method that resets all GA parameters
        private void GAResetButton_Click(object sender, EventArgs e)
        {
            this.GAReset_All();

            this.GA_Disable_Plot_Options();
            this.GAShowDataButton.Enabled = false;

            this.GAResetButton.Enabled = false;

            //managing panels
            this.outputPanel.Show();
            this.plotPanel.Hide();

            //clearing data in output panel
            this.clear_output_panel();

            //enabling launch button
            this.runGA.Enabled = true;

            //disabling excel button
            xlButton.Enabled = false;

        }

        //method that shows data in textboxes
        private void GAShowDataButton_Click(object sender, EventArgs e)
        {
            //hiding plots panle
            this.plotPanel.Hide();

            //showing output panel
            this.outputPanel.Show();
        }

        //method that clears output panel
        public void clear_output_panel()
        {
            //clearing data in output panel
            OutputbestFitness.Clear();
            outputAverageFitness.Clear();
            outputDispersion.Clear();

            //disabling output controls
            OutputbestFitness.Enabled = false;
            outputAverageFitness.Enabled = false;
            outputDispersion.Enabled = false;        
        }

        //method that enables output panel controls
        public void enable_output_panel_controls()
        {
            OutputbestFitness.Enabled = true; ;
            outputAverageFitness.Enabled = true;
            outputDispersion.Enabled = true; ;
            xlButton.Enabled = true;
        }

        #endregion

        //method that manages excel data
        public void xl_management()
        {
            //excel objects
            Excel._Application oXL;
            Excel._Workbook oWB;

            Excel._Worksheet bestSheet;
            Excel._Worksheet averageSheet;
            Excel._Worksheet dispersionSheet;

            Excel._Worksheet bestPlotSheet;
            Excel._Worksheet averagePlotSheet;
            Excel._Worksheet dispersionPlotSheet;

            //chart properties
            const string bestTitle = "Best Fitness Plot";
            const string averageTitle = "Average Fitness Plot";
            const string dispersionTitle = "Dispersion Plot";
            const string xAxis = "Time Steps";
            const string yAxis = "Fitness";

            string uniqueFileName = System.DateTime.Now.Ticks.ToString();
            string saveLocation = this.saving_directory + @"\data" + uniqueFileName + ".xls";    

            try
            {
                //get excel application object
                oXL = new Excel.Application();
                oXL.Visible = true;
                oXL.UserControl = true;

                //get a new workbook
                oWB = (Excel._Workbook)(oXL.Workbooks.Add());

                //creating sheets in workbook
                bestSheet = (Excel._Worksheet)oWB.Worksheets.get_Item(1);
                bestPlotSheet = (Excel._Worksheet)oWB.Worksheets.Add();
                bestSheet.Cells[1, 1] = "Best Fitness Data, (Each column contains data of each run)";

                averageSheet = (Excel._Worksheet)oWB.Worksheets.Add();
                averagePlotSheet = (Excel._Worksheet)oWB.Worksheets.Add();
                averageSheet.Cells[1, 1] = "Average Fitness Data (Each column contains data of each run)";
                
                dispersionSheet = (Excel._Worksheet)oWB.Worksheets.Add();
                dispersionPlotSheet = (Excel._Worksheet)oWB.Worksheets.Add();
                dispersionSheet.Cells[1, 1] = "Dispersion Data (Each column contains data of each run)";
                      
                //writing data to excel sheet
                for(int run=0; run<this.numberOfRuns; run++)
                {
                    for(int timeStep=1; timeStep<=this.numberOfTimeSteps; timeStep++)
                    {
                        bestSheet.Cells[timeStep+2, run + 2] = this.output[run].bestFitness[timeStep];
                        averageSheet.Cells[timeStep+2, run + 2] = this.output[run].averageFitness[timeStep];
                        dispersionSheet.Cells[timeStep+2, run + 2] = this.output[run].dispersion[timeStep];
                    }
                }

                //plotting data

                //creating charts
                var bestCharts = bestPlotSheet.ChartObjects() as Microsoft.Office.Interop.Excel.ChartObjects;
                var bestChartObject = bestCharts.Add(60,10,500,500) as Microsoft.Office.Interop.Excel.ChartObject;
                var bestChart = bestChartObject.Chart;

                var averageCharts = averagePlotSheet.ChartObjects() as Microsoft.Office.Interop.Excel.ChartObjects;
                var averageChartObject = averageCharts.Add(60, 10, 500, 500) as Microsoft.Office.Interop.Excel.ChartObject;
                var averageChart = averageChartObject.Chart;

                var dispersionCharts = dispersionPlotSheet.ChartObjects() as Microsoft.Office.Interop.Excel.ChartObjects;
                var dispersionChartObject = dispersionCharts.Add(10, 10, 500, 500) as Microsoft.Office.Interop.Excel.ChartObject;
                var dispersionChart = dispersionChartObject.Chart;

                //calculating last cell that has value for range calculation
                Excel.Range last = bestSheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Type.Missing);

                //setting range of values 
                var Range = bestSheet.get_Range("B3", last);
                //var averageRange = averageSheet.get_Range("B3", last);
                //var dispersionRange = dispersionSheet.get_Range("B3", last);

                //setting chart range
                bestChart.SetSourceData(Range);
                averageChart.SetSourceData(Range);
                dispersionChart.SetSourceData(Range);

                //setting chart properties
                bestChart.ChartType = Microsoft.Office.Interop.Excel.XlChartType.xlLine;
                bestChart.ChartWizard(Source: Range, Title: bestTitle, CategoryTitle: xAxis, ValueTitle: yAxis);

                averageChart.ChartType = Microsoft.Office.Interop.Excel.XlChartType.xlLine;
                averageChart.ChartWizard(Source: Range, Title: averageTitle, CategoryTitle: xAxis, ValueTitle: yAxis);

                dispersionChart.ChartType = Microsoft.Office.Interop.Excel.XlChartType.xlLine;
                dispersionChart.ChartWizard(Source: Range, Title: dispersionTitle, CategoryTitle: xAxis, ValueTitle: yAxis);

                //setting chart series names
                int i=1;
                foreach (var series in bestChart.SeriesCollection())
                {
                    series.Name = "Run" + i.ToString();
                    i++;
                }

                int j=1;
                foreach (var series in averageChart.SeriesCollection())
                {
                    series.Name = "Run" + j.ToString();
                    j++;
                }
                
                int k=0;
                foreach (var series in dispersionChart.SeriesCollection())
                {
                    series.Name = "Run" + k.ToString();
                    k++;
                }

                //saving file
                //string saveLocation = @"C:\Users\Aizaz\Desktop\file2.xls";
                oWB.SaveAs(saveLocation);
            }

            catch (Exception exception)
            {
                String errorMessage;
                errorMessage = "Error: ";
                errorMessage = String.Concat(errorMessage, exception.Message);
                errorMessage = String.Concat(errorMessage, " Line: ");
                errorMessage = String.Concat(errorMessage, exception.Source);

                MessageBox.Show(errorMessage, "Error");
            }
        }//end of method
            
        //method that is called when xl button is clicked
        private void xlButton_Click(object sender, EventArgs e)
        {
            //check if the user has set the directory to save the file
            if(string.IsNullOrEmpty(this.saving_directory))
            {
                System.Windows.Forms.MessageBox.Show("Please set the directory to save the file");
            }
            else
            {
                this.xl_management();
            }
            
        }

        //method that is called when back to main menu button is clicked
        private void button1_Click(object sender, EventArgs e)
        {
            //resetting all GA values
            this.GAReset_All();
            this.GA_Disable_Plot_Options();
            this.GAShowDataButton.Enabled = false;
            this.GAResetButton.Enabled = false;
            this.clear_output_panel();
            this.runGA.Enabled = true;
            xlButton.Enabled = false;

            //resetting all PSO values
            this.EnableAll();
            foreach (var series in chart1.Series)
            {
                series.Points.Clear();
            }
            foreach (var series in GAHigherRangeTextBox.Series)
            {
                series.Points.Clear();
            }
            foreach (var series in chart3.Series)
            {
                series.Points.Clear();
            }

            //managing panels
            this.showLaunchPanel();
        }

        //method that sets the directory to save the excel file
        private void saveButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.saving_directory = folderBrowserDialog1.SelectedPath.ToString();
            }
            
        }

        


      } //end of class

   }//end of namespace

