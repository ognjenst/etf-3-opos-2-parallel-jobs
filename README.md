Parallel Josb
---
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)
--

This was the second assignment for the "Selected Chapters from Operating Systems" subject: Parallel Jobs, using UWP.

Our task was to create a workload that would include parallelisation.
My idea was to go through an image and detect if it is a black and white image.

## Method
Image is represented as a 3D matrix of integers: width and height for each RGB component. If the value of each component is equal at the same position within representative matrixes, that pixel is considered to be a true black and white pixel.

So, I divided each image into several segments, and then use one thread per segment to check the pixels within. If only one pixel is off, from the condition, each image is labelled as false black and white image. In the end, only true B&W images will be saved in the folder, while the rest of the selected images, will be discarded from the memory. 
## Requirements
You will need [Visual Studio](https://visualstudio.microsoft.com/) to run this project.

## Configuration
You can select as many images as you like since the configuration is dynamic. I did tests up to 4k images, with no problem. 

## Improvements
There are a few possible improvements:
* Remove sleep method, and do the checks asynchronously.
* Connect the camera button, so that captured image can be processed. (Camera button was just added as a last minute option)
