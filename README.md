# PostureRecognition
This program uses Nuitrack SDK library to detect human body joints. Then input these data into a BiLSTM network to make posture prediction.

---
## 1 Real Sense
All instructions are based on `Real Sense 2.19.0` using `Depth Camera D435`.   

### 1-1 Viewer
- ### [Intel.RealSense.Viewer](https://github.com/IntelRealSense/librealsense/releases/download/v2.19.0/Intel.RealSense.Viewer.exe)
  + Executable depth camera control program
  + Configure depth camera and color camera parameters
  + Version: 2.19.0

### 1-2 SDK
- ### [Intel.RealSense.SDK](https://github.com/IntelRealSense/librealsense/releases/download/v2.19.0/Intel.RealSense.SDK.exe)
  + Installer with `Intel RealSense Viewer and Quality Tool`, `C/C++ Developer Package`, `Python 2.7/3.6 Developer Package`, `.NET Developer Package` and so on.
  + Version: 2.19.0

### 1-3 Extra Information
- Latest Version of Viewer and SDK: [Intel RealSense](https://github.com/IntelRealSense/librealsense/releases)
- https://realsense.intel.com/intel-realsense-downloads
- [Best Known Methods for Tuning Intel RealSense Depth Cameras D415 and D435](https://www.intel.com/content/dam/support/us/en/documents/emerging-technologies/intel-realsense-technology/BKMs_Tuning_RealSense_D4xx_Cam.pdf)

## 2 Nuitrack
All instructions are based on `NUITRACK 1.4.0`

### 2-1 Download
- SDK: [Nuitrack SDK](https://nuitrack.com/)  
  Support: Unity, Unreal Engine, C++, C# 
- Online Documents: [Nuitrack Online](http://download.3divi.com/Nuitrack/doc/)

### 2-2 Install
- [Installation Instructions ](http://download.3divi.com/Nuitrack/doc/Installation_page.html)   
  1. Download and run [nuitrack-windows-x64.exe](http://download.3divi.com/Nuitrack/platforms/nuitrack-windows-x64.exe) (for Windows 64-bit). Follow the instructions of the NUITRACK setup assistant. 
  2. Re-login to let the system changes take effect.
  3. [Required] Make sure that you have installed Microsoft Visual C++ Redistributable for Visual Studio on your computer. If not, install this package depending on your VS version and architecture:   
      + [Visual C++ Redistributable 2015 (x64)](https://download.microsoft.com/download/9/3/F/93FCF1E7-E6A4-478B-96E7-D4B285925B00/vc_redist.x64.exe)
      + [Visual C++ Redistributable 2017 (x64)](https://aka.ms/vs/15/release/VC_redist.x64.exe)

### 2-3 Examples
-   [nuitrack_console_sample/src/main.cpp](http://download.3divi.com/Nuitrack/doc/nuitrack_console_sample_2src_2main_8cpp-example.html)
-   [nuitrack_csharp_sample/Program.cs](http://download.3divi.com/Nuitrack/doc/nuitrack_csharp_sample_2Program_8cs-example.html) :star:
-   [nuitrack_gl_sample/src/main.cpp](http://download.3divi.com/Nuitrack/doc/nuitrack_gl_sample_2src_2main_8cpp-example.html)
-   [nuitrack_ni_gl_sample/src/main.cpp](http://download.3divi.com/Nuitrack/doc/nuitrack_ni_gl_sample_2src_2main_8cpp-example.html)
