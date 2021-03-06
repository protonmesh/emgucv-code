# --------------------------------------------------------
#  Copyright (C) 2004-2014 by EMGU Corporation. All rights reserved.
# --------------------------------------------------------

PROJECT(Example.SURFFeature)

FILE(GLOB_RECURSE SRC_Example_SURFFeature RELATIVE ${CMAKE_CURRENT_SOURCE_DIR} "*.cs")
LIST(APPEND SRC_Example_SURFFeature ${CMAKE_CURRENT_SOURCE_DIR}/../../CommonAssemblyInfo.cs)

#IF(TARGET_ARCH_64)
#  LIST(APPEND CS_FLAGS -platform:x64)
#ELSE()
#  LIST(APPEND CS_FLAGS -platform:x86)
#ENDIF()

LIST(APPEND CS_FLAGS ${EMGU_CV_REFERENCE} ${EMGU_CV_CUDA_REFERENCE} ${EMGU_CV_NONFREE_REFERENCE})

#copy the test image for this example
ADD_CS_FILE_TO_DEPLOY(${CMAKE_CURRENT_SOURCE_DIR}/box.png)
ADD_CS_FILE_TO_DEPLOY(${CMAKE_CURRENT_SOURCE_DIR}/box_in_scene.png)

COMPILE_CS(${PROJECT_NAME} "winexe" "${SRC_Example_SURFFeature}" ALL)
set_target_properties(${PROJECT_NAME} PROPERTIES FOLDER "Emgu.CV.Examples")
ADD_DEPENDENCIES(${PROJECT_NAME} Emgu.CV Emgu.CV.UI Emgu.CV.Cuda Emgu.CV.Nonfree)

INSTALL(FILES 
  ${LIBRARY_OUTPUT_PATH}/${PROJECT_NAME}.${CS_EXECUTABLE_EXTENSION} 
  ${CMAKE_CURRENT_SOURCE_DIR}/box.png
  ${CMAKE_CURRENT_SOURCE_DIR}/box_in_scene.png
  DESTINATION bin COMPONENT emgucv_example_binary)
