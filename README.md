# PostureRecognition
This program uses Nuitrack SDK library to detect human body joints. Then input these data into a BiLSTM network to make posture prediction.

---
## 1 Real Sense
All instructions are based on `Real Sense 2.19.0` using `Depth Camera D435`.   

## 1-1 Viewer
- ### [Intel.RealSense.Viewer](https://github.com/IntelRealSense/librealsense/releases/download/v2.19.0/Intel.RealSense.Viewer.exe)
  + Executable depth camera control program
  + Configure depth camera and color camera parameters
  + Version: 2.19.0

## 1-2 SDK
- ### [Intel.RealSense.SDK](https://github.com/IntelRealSense/librealsense/releases/download/v2.19.0/Intel.RealSense.SDK.exe)
  + Installer with `Intel RealSense Viewer and Quality Tool`, `C/C++ Developer Package`, `Python 2.7/3.6 Developer Package`, `.NET Developer Package` and so on.
  + Version: 2.19.0

## 1-3 Extra Information
- Latest Version of Viewer and SDK: [Intel RealSense](https://github.com/IntelRealSense/librealsense/releases)
- https://realsense.intel.com/intel-realsense-downloads
- [Best Known Methods for Tuning Intel RealSense Depth Cameras D415 and D435](https://www.intel.com/content/dam/support/us/en/documents/emerging-technologies/intel-realsense-technology/BKMs_Tuning_RealSense_D4xx_Cam.pdf)
