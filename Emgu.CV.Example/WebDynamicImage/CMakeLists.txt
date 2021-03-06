# --------------------------------------------------------
#  Copyright (C) 2004-2014 by EMGU Corporation. All rights reserved.
# --------------------------------------------------------

PROJECT(Example.WebDynamicImage)

###################################################################
# Create the Bin folder if it is not already exists
###################################################################
SET(WEB_DYNAMIC_IMAGE_BIN_FOLDER "${CMAKE_CURRENT_SOURCE_DIR}/Bin${UNMANAGED_LIBRARY_OUTPUT_SUBFOLDER}")
SET(APP_DATA_FOLDER "${CMAKE_CURRENT_SOURCE_DIR}/App_Data")

IF(NOT(EXISTS ${WEB_DYNAMIC_IMAGE_BIN_FOLDER} AND IS_DIRECTORY ${WEB_DYNAMIC_IMAGE_BIN_FOLDER}))
   FILE(MAKE_DIRECTORY ${WEB_DYNAMIC_IMAGE_BIN_FOLDER})
ENDIF()
IF(NOT(EXISTS ${APP_DATA_FOLDER} AND IS_DIRECTORY ${APP_DATA_FOLDER}))
   FILE(MAKE_DIRECTORY ${APP_DATA_FOLDER})
ENDIF()

#SET(TMP opencv_core opencv_imgproc opencv_highgui opencv_objdetect opencv_ml opencv_calib3d opencv_contrib opencv_features2d opencv_legacy opencv_video opencv_flann cvextern )
FOREACH(tmp_component ${OPENCV_LIBRARY_OUTPUT_NAMES})
   LIST(APPEND WEB_DYNAMIC_IMAGE_COPY_COMMAND 
     COMMAND ${CMAKE_COMMAND} -E copy "${UNMANAGED_LIBRARY_OUTPUT_PATH}/${tmp_component}.dll" "${WEB_DYNAMIC_IMAGE_BIN_FOLDER}/${tmp_component}.dll")
ENDFOREACH()
#MESSAGE("-----------CVEXTERN_DEPENDENCY_DLL_NAMES: ${CVEXTERN_DEPENDENCY_DLL_NAMES}")
FOREACH(tmp_component ${CVEXTERN_DEPENDENCY_DLL_NAMES})
   LIST(APPEND WEB_DYNAMIC_IMAGE_COPY_COMMAND 
     COMMAND ${CMAKE_COMMAND} -E copy "${UNMANAGED_LIBRARY_OUTPUT_PATH}/${tmp_component}.dll" "${WEB_DYNAMIC_IMAGE_BIN_FOLDER}/${tmp_component}.dll")
ENDFOREACH()

MESSAGE(STATUS "WEB Dynamic Image Copy Command:${WEB_DYNAMIC_IMAGE_COPY_COMMAND}")

LIST(APPEND CS_FLAGS -r:"${LIBRARY_OUTPUT_PATH}/Emgu.CV.OCR.dll")

FILE(GLOB TESSDATA "${CMAKE_CURRENT_SOURCE_DIR}/../../Emgu.CV.Extern/tesseract/libtesseract/tesseract-ocr/tessdata/eng.*")
MESSAGE(STATUS "TESSDATA: ${TESSDATA}")
FOREACH(TESSDATA_FILE ${TESSDATA})
  GET_FILENAME_COMPONENT(TESSDATA_FILE_NAME ${TESSDATA_FILE} NAME)
  LIST(APPEND WEB_DYNAMIC_IMAGE_COPY_COMMAND 
     COMMAND ${CMAKE_COMMAND} -E copy "${TESSDATA_FILE}" "${CMAKE_CURRENT_SOURCE_DIR}/App_Data/tessdata/${TESSDATA_FILE_NAME}")
ENDFOREACH()

ADD_CUSTOM_TARGET(
   ${PROJECT_NAME} ALL
   ${WEB_DYNAMIC_IMAGE_COPY_COMMAND}
)
set_target_properties(${PROJECT_NAME} PROPERTIES FOLDER "Emgu.CV.Examples")
ADD_DEPENDENCIES(${PROJECT_NAME} Emgu.CV Emgu.CV.UI Emgu.CV.OCR)


