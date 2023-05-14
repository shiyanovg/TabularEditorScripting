### What-If Parameter With TabularEditor

This repo is about creating What-If Parameters with Tabular Editor. 
Scenarios in which you may need this solution include:

- You have datasets published on SSAS On-Premises/Azure Analysis Services/Power BI Premium
- You store you Model in a folder structure rather than ***Model.bim*** file
- You [deploy your folder structure](https://www.youtube.com/watch?v=fzZgXe3MjhI) with [Azure DevOps](https://azure.microsoft.com/en-us/products/devops)

The full code is available [here](./What_If_Parameter_With_TabularEditor.csx).


Here are the steps:
1. Copy the code from *.csx file
![Step 1](./img/Step1.png)

2. Open your solution in Tabular Editor
![Step 2](./img/Step2.png)

3. Paste the code into Tabular Editor ***C# Script*** window
![Step 3](./img/Step3_NEW.png)

4. Run macro and follow the steps in it

	4.1 Configure the What-If Parameter just like you would in Power BI
    ![Step Macro 1](./img/Script_step_1.png)

	4.2 Check results of the script
	![Step Macro 2](./img/Script_step_2.png)
    
	4.3 Save model back to the folder structure (or ***Model.bim*** file) and deploy the model to your environment (with Model->Deploy)
	![Step Macro 3](./img/Script_step_3.png)


5. Now it's time to process our changes

	5.1. Open and process your model in SQL Server Management Studio
	![SSMS Step 1](./img/SSMS_Step_1_NEW.png)
	
	5.2. While processing, choose ***Process Recalc***. (also known as *calculate*)
    ```
	{
	  "refresh": {
		"type": "calculate",
		"objects": [
		  {
			"database": "What-If-Parameter_With_TabularEditor"
		  }
		]
	  }
	}
	```
	![SSMS Step 2](./img/SSMS_Step_2_NEW.png)
	
	5.3. Wait for process to finish
	![SSMS Step 3](./img/SSMS_Step_3_NEW.png)


6. Check that model works as expected.

	6.1. With DAX Studio
	![Check with DAX Studio](./img/Check_With_DAX-Studio.png)

	6.2. With Power BI Desktop
	
	Default value:
	![Check With Power BI Desktop 1](./img/Check_With_PBI_Step_1.png)
	Slicer value:
	![Check With Power BI Desktop 2](./img/Check_With_PBI_Step_2.png)


