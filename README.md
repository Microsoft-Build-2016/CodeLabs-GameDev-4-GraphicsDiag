<a name="HOLTop" ></a>
# Graphics Diagnostics and Performance Tuning for Games with VS2015 #

---

<a name="Overview" ></a>
## Overview ##

There are many things that can go wrong when developing game, including memory issues, rendering issues, strange behaviors, performance issues, hardware issues and much more. In this session, you will get hands on experience with the **Visual Studio Graphics Diagnostics tools**  to help you in situations where your game needs analysis and debugging. You will learn how the tools work, and how to use them to solve different graphical issues.

<a name="Objectives" ></a>
### Objectives ###
In this module, you will:

- Use the CPU Profiling tools to identify performance issues
- Use the Frame Capture tool and shader debugger to identify errors in the shaders
- Use the Event view to see drawcalls and possibilities for performane optimization

<a name="Prerequisites"></a>
### Prerequisites ###

The following is required to complete this module:

- [Visual Studio Community 2015][1] or greater
- [VS Graphics Diagnostics Tools][2]

[1]: https://www.visualstudio.com/products/visual-studio-community-vs
[2]: https://msdn.microsoft.com/en-us/library/mt125501.aspx

---

<a name="Exercises"></a>
## Exercises ##
This module includes the following exercises:

1. [Profiling](#Exercise1)
1. [Shader and Graphic debugging](#Exercise2)
1. [Events and Pipeline Stages](#Exercise3)

Estimated time to complete this module: **60 minutes**

<a name="Exercise1"></a>
### Exercise 1: Profiling ###

<a name="Ex1Task1"></a>
#### Task 1 - Welcome! ####

1. Open the project in UNITY and play the game a few times by clicking on the **PLAY** button. You control the rocket by pressing **A** and **D** on the keyboard, and launch by pressing **SPACE** or click the **Launch** button on the user interface. Above the rocket, there is a path indicator:

    ![Path indicator](Images/image001.png?raw=true "Path indicator")
    
    _Path indicator_

1. Your job is to keep the red dot as centered as possible (so the offset reading below the marker stays as close to 0.00 as possible). There is an in game line that also will show where the path goes:

    ![Path line](Images/image003.png?raw=true "Path line")
    
    _Path line_

1. The launch conditions are windy, and the wind will try to push you off the path. Use **A** and **D** to slowly rotate the rocket back on track.

    One round lasts for about 30 seconds, and you will be measured by how close you are to the path when you cross the finish line.

    ![Game finish](Images/image005.png?raw=true "Game finish")
    
    _Game finish_


<a name="Ex1Task2"></a>
#### Task 2 - Exporting the game as a UWP ####
Now that we know how the game works, we want to run it through a performance analysis to see if we are having any performance issues.

1. In Unity, click **File->Build Settings**

    ![File menu](Images/image007.png?raw=true "File menu")
    
    _File menu_

1. Select **Windows Store**, and from the dropdown, select **Universal 10**.

    ![Build Settings](Images/image008.png?raw=true "Build Settings")
    
    _Build Settings_

1. Make sure you also tick the **Unity C# Projects** and the **Development Build** check-boxes.

    ![Windows Store build settings](Images/image009.png?raw=true "Windows Store build settings")
    
    _Windows Store build settings_

1. Next, click **Build** and create a subfolder in the rood project named **EXPORT**, and another one inside the new EXPORT folder called **UWP** and click **Select Folder** to start the export.

    ![Selecting build folder](Images/image011.png?raw=true "Selecting build folder")
    
    _Selecting build folder_

<a name="Ex1Task3"></a>
#### Task 3 - Opening the Visual Studio solution ####

1. Once the export is done, the new folder should automatically open. Open **Apollo.sln**.

    ![Exported folder](Images/image013.png?raw=true "Exported folder")
    
    _Exported folder_

1. Once Visual Studio loads the project, change the build configuration to **x86**, and make sure that it is in **Debug**. Click the **Local Machine** button to build, deploy and run the solution and test that it works:

    ![Build and run](Images/image015.png?raw=true "Build and run")
    
    _Build and run_

	> **Note:** Some build error messages might appear since this is the first time we are building the solution. This is normal as long as the build process is continuing.

1. The game is a bit slow and laggy. A partial reason is that we are running under the debugger on a Debug build, but let's see what happens behind the scenes.

1. Stop the game.


<a name="Ex1Task4"></a>
#### Task 4 - Running the Profiler ####

1. Click **Debug->Start Diagnostics Tools Without Debugging...**

    ![Start Diagnostics Tools Without Debugging](Images/image016.png?raw=true "Start Diagnostics Tools Without Debugging")
    
    _Start Diagnostics Tools Without Debugging_

1. Run the game with only the **CPU Usage** enabled. If the GPU profiler is enabled, tick that off since we will only be using the CPU Profiler.

    ![GPU Usage only](Images/image017.png?raw=true "GPU Usage only")
    
    _GPU Usage only_

1. Next, click **Start** to run the game with the profiler:

    ![Start button](Images/image019.png?raw=true "Start button")
    
    _Start button_

1. Do a launch and let the game run for a few seconds before clicking the **STOP** button.

    ![Running and stop button](Images/image021.png?raw=true "Running and stop button")
    
    _Running and stop button_

    The _Graphics Diagnostics_ tools will gather some information. 


<a name="Ex1Task5"></a>
#### Task 5 - Checking the CPU Profiler ####

1. Go to the **CPU Usage** tab below the graphs.

    ![CPU Usage](Images/image027.png?raw=true "CPU Usage")
    
    _CPU Usage_

1. Check the CPU usage and notice that the **RocketControllers::Update** function is using a lot of CPU usage, in this case _48.03%_.

1. Open the **RocketController::$Invoke16** tree to dive a bit deeper.

    ![Functions CPU usage](Images/image029.png?raw=true "Functions CPU usage")
    
    _Functions CPU usage_

1. Going into it, we can see that it's external code that's spending most of it, and that external code might not have any debugging information. Also, notice the **WindRandomnesGenerator** function call here. It's quite low on the CPU Usage, but I know it's calling external code so this might be the source of error.

1. Let's do some more digging by clicking **Create detailed report**:

    ![Create detailed report](Images/image031.png?raw=true "Create detailed report")
    
    _Create detailed report_

1. This will give some some more details around what we are doing, and you will see a similar screen to this:

    ![Detailed report](Images/image033.png?raw=true "Detailed report")
    
    _Detailed report_

	There is a lot of details on this screen, but for now, notice the **Hot Path** tree, and a call to **Mathf.PerlinNoise**. In this wind generator, we're using multiple calls to the **PerlinNoise** function to generate a pseudo-random wind pattern using various octaves of perlin noise added together.


<a name="Ex1Task6"></a>
#### Task 6 - Checking the function ####

1. Going back to Unity, open the script for our **RocketController**:

    ![RocketController script](Images/image035.png?raw=true "RocketController script")
    
    _RocketController script_

	By looking at the various functions in this short script, can see that most of the functions in the Update loops are pretty straight forward except for the _WindRandomnessGenerator()_ function we identified above.

1. To ensure we are looking at the right performance issue, let's first try to comment out the line of code that is generating the wind. Go to the _Update()_ function in the _RocketController_ script, and comment the first line of code:

    ![Commenting out WindRandomnessGenerator call](Images/image037.png?raw=true "Commenting out WindRandomnessGenerator call")
    
    _Commenting out WindRandomnessGenerator call_

1. Now, save the change and do the export again from Unity. Once done, do a rebuild all:

    ![Rebuild Solution](Images/image039.png?raw=true "Rebuild Solution")
    
    _Rebuild Solution_

1. Run the game under CPU profiler like we did earlier:

    ![Start Diagnostics Tools Without Debugging](Images/image016.png?raw=true "Start Diagnostics Tools Without Debugging")
    
    _Start Diagnostics Tools Without Debugging_

1. Go the the CPU Usage tab and notice the changed values. The Update function is now at normal levels. 

    ![Improved CPU usage](Images/image041.png?raw=true "Improved CPU usage")
    
    _Improved CPU usage_


<a name="Ex1Task7"></a>
#### Task 7 - Fixing it ####

1. Uncomment the call to our wind generator:

    ![Wind generator call](Images/image043.png?raw=true "Wind generator call")
    
    _Wind generator call_

    It's calculating way too many octaves on the _Perlin Noise_ function, one per increase in view while in our intention, we just wanted to have a few octaves with a good range between each octave.

1. This must be changed. Change the jump in octaves to **20000**, significantly reducing the number of calls. The code should look like this:

	````C#
	float WindRandomnessGenerator()
	{
		 float windRandomness = 0.0f;
		 int octaves = 0;
		 int lastOctaveDistance = 100000;
		 int increaseDistancePerOctave = 20000;

		 for (int i = 0; i < lastOctaveDistance; i += increaseDistancePerOctave)
		 {
			  octaves++;
			  windRandomness += Mathf.PerlinNoise(
					transform.position.x / (lastOctaveDistance - i) + 1.0f,
					transform.position.y / (lastOctaveDistance - i) + 1.0f);
		 }

		 return windRandomness / octaves;
	}
	````

1. Export the game again, and run it under the CPU profile like in Task 6.

The issue is now fixed, and we now got a better framerate! However, we have more graphics issues to fix so let's move on to the next exercise!


<a name="Exercise2"></a>
### Exercise 2: Shader and Graphic debugging ###

<a name="Ex2Task1"></a>
#### Task 1 - Identify issue ####

1. Open the **Begin** project in Unity and play another round of the game. Focus on the moon, and notice that it looks a bit weird.

	The VS Graphics Diagnostics tools comes with a set of tools that enables you to see all the drawcalls as well as the history of selected pixels, as well as shader debugging and shader editing.

<a name="Ex2Task2"></a>
#### Task 2 - Export as a UWP ####

1. Let's go ahead and build the solution. In Unity, click **File->Build Settings**.

    ![File menu](Images/image007.png?raw=true "File menu")
    
    _File menu_

1. Ensure that **Windows Store**, and from the dropdown, **Universal 10** is selected.

    ![Build settings](Images/image008.png?raw=true "Build settings")
    
    _Build settings_

1. Also, make sure the Unity C# Projects and the Development Build checkboxes is checked.

    ![Windows Store build settings](Images/image009.png?raw=true "Windows Store build settings")
    
    _Windows Store build settings_

1. Click **Build** and navigate into **EXPORT\UWP** and click **Select Folder** to start the export.

    ![Selecting build folder](Images/image011.png?raw=true "Selecting build folder")
    
    _Selecting build folder_


<a name="Ex2Task3"></a>
#### Task 3 - Opening the Visual Studio solution ####

1. Once the export is done, the new folder should automatically open. Open the Apollo.sln.

    ![Exported folder](Images/image013.png?raw=true "Exported folder")
    
    _Exported folder_

1. Once Visual Studio loads the project, change the build configuration to **x86**, and make sure that it is in **Debug**. Click the **Local Machine** button to build, deploy and run the solution and test that it works:

    ![Build and run](Images/image015.png?raw=true "Image")
    
    _Build and run_

	> **Note:** Some build error messages might appear since this is the first time we are building the solution. This is normal as long as the build process is continuing.

1. Feel free to try the game and ensure that the moon is looking strange.

1. Stop the game.
 

<a name="Ex2Task4"></a>
#### Task 4 - Run in Graphics Debug mode ####

1. Let's run this with the _Graphics Diagnostics_ by clicking **Debug->Graphics->Start Diagnostics**.

    ![Start Diagnostics](Images/image048.png?raw=true "Start Diagnostics")
    
    _Start Diagnostics_

1. When running the game now, you are presented with performance graphs, as well as a _Capture Frame_ button:

    ![Running with diagnostics](Images/image050.png?raw=true "Running with diagnostics")
    
    _Running with diagnostics_

1. To capture a frame, you can click the **Capture Frame** button, or press **Print screen** on the keyboard when having the app in focus.

   We are interested in finding out what's happening with the moon. When the game starts, it's stationary on the Launchpad, and the broken white moon is visible in the background. 

1. When the moon is visible, capture a frame by clicking the button or using Print screen.

1. Then, when the frame is captured, you can see it visible where the capture frame button was.

1. Stop the session by clicking the **Stop** button:

    ![Stop button](Images/image052.png?raw=true "Stop button")
    
    _Stop button_

1. Once the app stops, you can click the captured frame by clicking on the frame header:

    ![Captured frame](Images/image054.png?raw=true "Captured frame")
    
    _Captured frame_

1. A new tab will open in visual studio, containing a snapshot of the system, processes, state of the app at that time the frame was captured.

    ![Captured system snapshot](Images/image055.png?raw=true "Captured system snapshot")
    
    _Captured system snapshot_

<a name="Ex2Task5"></a>
#### Task 5 - Navigating around in the captured frame ####

1. On the left side, there is a long list of events like draw calls, and in the center we are having a picture that shows the state of the frame at a selected event.

<a name="Ex2Task6"></a>
#### Task 6 - Finding the moon ####

1. We are interested in finding the draw call that draws our moon. Click the last event before the Present event to be sure that we are having a preview that has the moon visible.

    ![Last event before present](Images/image057.png?raw=true "Last event before present")
    
    _Last event before present_

1. Now, on the frame view, click a pixel inside of the moon. A red marker will show what pixel you have selected:

    ![Selected moon's pixel](Images/image059.png?raw=true "Selected moon's pixel")
    
    _Selected moon's pixel_

1. One the right side of this, in the properties view, you can see all the events that are related to this pixel:

    ![Pixel related events](Images/image061.png?raw=true "Pixel related events")
    
    _Pixel related events_

1. The first one is the one that clears the screen to blue, the next draws the moon and the last draws the skybox. You can click each of these to see how it looks in the frame preview.

1. Open the event by clicking on the arrow of the event in the pixel history, and open the Triangle header. This will display information about how the pixel is affected, what shaders are being used and what color the various stages produce.

    ![Pixel info](Images/image063.png?raw=true "Pixel info")
    
    _Pixel info_

<a name="Ex2Task7"></a>
#### Task 7 - Checking the graphics pipeline ####

1. Below the preview, all the way to the bottom of the screen, there are a few tabs where one is gamed **Pipeline Stages**. Click this:

    ![Pipeline Stages](Images/image065.png?raw=true "Pipeline Stages")
    
    _Pipeline Stages_

    This will show how all the pipeline stages the selected event is having. This one is having one Input stage where all the vertices that are being used is passed into the shader stages. The next is a vertex shader, that is executed once per vertex from the input stage, then there is a pixel shader that gives a color to every pixel, and then finally an output merger that marks the end of the graphics pipeline.

1. If you click on one of the stages, like the **Input Assembler**, you can see how the model currently in process looks like in the built in _Visual Studio Model Viewer_. You can see that this is a sphere, from what the moon is built up from.

    ![Moon mesh preview](Images/image067.png?raw=true "Moon mesh preview")
    
    _Moon mesh preview_

    By the looks of it, the sphere looks right so the issue will probably be in the Pixel Shader where we give it color.


<a name="Ex2Task8"></a>
#### Task 8 - Debug shader ####

Now, below each of the stages in the _Pipline Stage_ view, or in the _Pixel History_, you can see what shader is being used, as well as a green play button. By clicking on the shader name, you can see the code of the shader. This shader will create a rim around the moon, but by the looks of it, the rim color seems to be in the center, inverse.

1. We are interested in debugging it, so click the **green play button** next to the _Pixel Shade_r (the one underlined below):

    ![Debug pixel shader](Images/image069.png?raw=true "Debug pixel shader")
    
    _Debug pixel shader_

1. The shader code will become visible, with the possibility to step through the shader:

    ![Debugging shader code](Images/image070.png?raw=true "Debugging shader code")
    
    _Debugging shader code_

1. Set a breakpoint at the last line in the shader and step through until you reach this line. Look at the content of the variable named rim.

    ![Shader breakpoint](Images/image072.png?raw=true "Shader breakpoint")
    
    _Shader breakpoint_

1. You can use the tools above the tab to step through the shader:

    ![Step through tools](Images/image073.png?raw=true "Step through tools")
    
    _Step through tools_

    Looking at the calculations of the rim value, we have forgotten to invert it by subtracting it from one. Lets try this, so go ahead and stop the debugger and open the shader for editing.
 

<a name="Ex2Task9"></a>
#### Task 9 - Edit the shader ####

1. Click on the shader tab and edit the code so it looks like this:

    ![Fixing shader](Images/image075.png?raw=true "Fixing shader")
    
    _Fixing shader_

    The only change is to write `1.0f -` in front of saturate at the line that calculates the rim effect.

1. Save it, and the _Frame Capture_ will update itself to use this shader. Click the frame preview tab, and notice that the moon is now fixed!

    ![Fixed moon](Images/image077.png?raw=true "Fixed moon")
    
    _Fixed moon_

<a name="Ex2Task10"></a>
#### Task 10 - Fix issue in Unity and re-export ####

1. Close the debugging session and go back to Unity, and find the **Velvet** shader we are using by navingating to the **Shaders/Velvet** folder:

    ![Velvet shader](Images/image079.png?raw=true "Velvet shader")
    
    _Velvet shader_

1. Open this in _Visual Studio_ and make the same change to it, add `1.0f -` in front of saturate.

	```C#
	void surf (Input IN, inout SurfaceOutput o) {

		 o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;

		 o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));

		 half rim = 1.0f - saturate(dot (normalize(IN.viewDir), o.Normal));

		 o.Emission = _RimColor.rgb * pow (rim, _RimPower);

	}
	```

1. Save the change.

1. Play the game from the Unity Editor, and see that it now works well, and the moon is fixed!

<a name="Exercise3"></a>
### Exercise 3: Events and Pipeline Stages ###

<a name="Ex3Task1"></a>
#### Task 1 - Export the UWP ####

By this stage, we are already familiar with how the game works and how to export the game from Unity and attach the _Graphics Diagnostic_ Tools. The aim here is to learn how the _Event List_ works, and how you can use it.

1. Export the game from Unity. In Unity, click **File->Build Settings**:

    ![File menu](Images/image007.png?raw=true "File menu")
    
    _File menu_

1. Ensure that **Windows Store**, and from the dropdown, **Universal 10** is selected. Also, make sure the **Unity C# Projects** and the **Development Build** checkboxes is checked.

    ![Windows Store build settings](Images/image009.png?raw=true "Windows Store build settings")
    
    _Windows Store build settings_

1. Click Build and navigate into **EXPORT\UWP** and click **Select Folder** to start the export.

    ![Selecting build folder](Images/image011.png?raw=true "Selecting build folder")
    
    _Selecting build folder_

<a name="Ex3Task2"></a>
#### Task 2 - Opening the Visual Studio solution ####

1. Once the export is done, the new folder should automatically open. Open the **Apollo.sln**.

    ![Exported folder](Images/image013.png?raw=true "Exported folder")
    
    _Exported folder_

1. Once Visual Studio loads the project, change the build configuration to **x86**, and make sure that it is in **Debug**. Click the **Local Machine** button to build, deploy and run the solution and test that it works:

    ![Build and run](Images/image015.png?raw=true "Build and run")
    
    _Build and run_

<a name="Ex3Task3"></a>
#### Task 3 - Run in Graphics Debug mode ####

1. Let's run this with the _Graphics Diagnostics_ by clicking **Debug->Graphics->Start Diagnostics**.

    ![Start Diagnostics](Images/image048.png?raw=true "Start Diagnostics")
    
    _Start Diagnostics_

1. When running the game now, you are presented with performance graphs, as well as a _Capture Frame_ button:

    ![Running with diagnostics](Images/image050.png?raw=true "Running with diagnostics")
    
    _Running with diagnostics_

1. To capture a frame, you can click the Capture Frame button, or press Print screen on the keyboard when having the app in focus.

1. Stop the session by clicking the **Stop** button:

    ![Stop button](Images/image052.png?raw=true "Stop button")
    
    _Stop button_

1. Once the app stops, you can click the captured frame by clicking on the frame header:

    ![Captured frame](Images/image054.png?raw=true "Captured frame")
    
    _Captured frame_

1. A new tab will open in visual studio, containing a snapshot of the system, processes, state of the app at that time the frame was captured.

    ![Captured system snapshot](Images/image055.png?raw=true "Captured system snapshot")
    
    _Captured system snapshot_

1. On the left side, you can see a long list of events. You can click these events to see how the frame looks at that event. 


<a name="Ex3Task4"></a>
#### Task 4 - Finding the right clear event ####

1. Let's try this. Scroll up to a bit over the middle of the list view, and you will see an event that clears the screen to a given color:

    ![Clear screen event](Images/image087.png?raw=true "Clear screen event")
    
    _Clear screen event_

1. Clicking this will show how the frame looks to the right of the list (in the preview):

    ![Frame render](Images/image089.png?raw=true "Frame render")
    
    _Frame render_

    If you follow the events down, clicking the one by one, you will see that events draw what part of the frame like the terrain, the launchpad, the rocket, the particles, the UI and so on.

    If you do this one by one (don't do this), you can see the frame being built up slowly, piece by piece.

<a name="Ex3Task5"></a>
#### Task 5 - Finding the draw calls that belong to our tower ####

1. By clicking through the event view, you will first see that a lot of them belong to the terraing, and then another bunch that belong to the tower.

1. Another way of finding where about the tower draw events are, is by using the _pixel history_ (as we used in Exercise 2)

1. In the frame view, click on a pixel on the Launchpad tower to view the pixel history:

    ![Launchpad tower's pixel](Images/image091.png?raw=true "Launchpad tower's pixel")
    
    _Launchpad tower's pixel_

1. You can see that there are many events that are affecting the pixel:

    ![Events for lunchpad tower's pixel](Images/image093.png?raw=true "Events for lunchpad tower's pixel")
    
    _Events for lunchpad tower's pixel_

1. Select one by one, starting from the top to find one that belongs to the tower:

    ![Selecting event](Images/image095.png?raw=true "Selecting event")
    
    _Selecting event_

1. In this case, there are many that belong to the tower. You can identify this by seeing that a part of the tower has been drawn on the frame view, and if you select the **Pipeline Stages** tab, you can see the part that is currently being drawn:

    ![Pipeline Stages tab](Images/image097.png?raw=true "Pipeline Stages tab")
    
    _Pipeline Stages tab_

<a name="Ex3Task6"></a>
#### Task 6 - Finding all the draw calls for our tower ####

1. Once you have found one part that belongs to the tower, move upwards to you find the first drawcall for the tower, and move down until the tower has been completely drawn.

    ![Tower draw](Images/image099.png?raw=true "Tower draw")
    
    _Tower draw_

    The last part is found here:

    ![Last tower draw event](Images/image101.png?raw=true "Last tower draw event")
    
    _Last tower draw event_

    As you can see, there is a lot of draw calls for making the tower. Sometimes this is ok and necessary, but in this case, we are drawing parts of the model multiple times, with the same shader and graphics pipeline stage.

<a name="Ex3Task7"></a>
#### Task 7 - Reducing the draw calls of our tower ####

1. The designer of the launchtower model created another version that got the tower parts combined into one mesh. This will reduce the drawcall we had for each floor of the tower and so on.

1. In the **End** folder, the new model has replaced the existing one.

<a name="Ex3Task8"></a>
#### Task 8 - Test ####

1. You can directly open the exported UWP of the End project, run it and capture a frame.

1. You can now see that the most of the tower is rendered in only a couple of drawcalls. 

<a name="summary" />
## Summary ##

Thank you for going through this module. We hope that by going through these steps, you have learned how you can use the Diagnostics Tools in your own graphical projects. There are many other problems in this solution, so you you want to go even further, feel free to spend some more time with it.
