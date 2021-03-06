# --------------------------------------------------------
#  Copyright (C) 2004-2014 by EMGU Corporation. All rights reserved.
# --------------------------------------------------------

IF((EMGU_CV_BUILD OR EMGU_CV_EXAMPLE_BUILD) AND NOT ANDROID)

PROJECT(Emgu.Util)

FILE(GLOB_RECURSE SRC_EMGU_UTIL RELATIVE ${CMAKE_CURRENT_SOURCE_DIR} "*.cs")
LIST(APPEND SRC_EMGU_UTIL ${CMAKE_CURRENT_SOURCE_DIR}/../CommonAssemblyInfo.cs)
#IF(NETFX_CORE)
#ELSE()
#  ADD_CS_REFERENCES("System.Xml.Linq.dll")
#ENDIF()
FILE(GLOB_RECURSE SRC_EMGU_UTIL_EXCLUDES RELATIVE ${CMAKE_CURRENT_SOURCE_DIR} "MonoTouch/*.cs" "Android/*.cs")
LIST(REMOVE_ITEM SRC_EMGU_UTIL ${SRC_EMGU_UTIL_EXCLUDES})

GENERATE_DOCUMENT(${LIBRARY_OUTPUT_PATH}/${PROJECT_NAME})

SIGN_ASSEMBLY(${CMAKE_CURRENT_SOURCE_DIR}/../Emgu.CV.snk)
SET_CS_TARGET_FRAMEWORK(${CSC_TARGET_FRAMEWORK})
COMPILE_CS(${PROJECT_NAME} "library" "${SRC_EMGU_UTIL}" ALL)

if(ENABLE_SOLUTION_FOLDERS)
    set_target_properties(${PROJECT_NAME} PROPERTIES FOLDER "emgu")
endif()

INSTALL(
  FILES 
  ${LIBRARY_OUTPUT_PATH}/${PROJECT_NAME}.xml
  ${LIBRARY_OUTPUT_PATH}/${PROJECT_NAME}.dll 
  DESTINATION ${CPACK_PACKAGE_CLI_FOLDER}
  COMPONENT emgucv_binary)
  
IF(WIN32 OR APPLE)

IF(NETFX_CORE)
  SET(CSPROJ_FILE "${PROJECT_NAME}.Windows.Store.csproj")
ELSE()
  SET(CSPROJ_FILE "${PROJECT_NAME}.csproj")
ENDIF()

INSTALL(
  DIRECTORY
  ${CMAKE_CURRENT_SOURCE_DIR}
  DESTINATION .
    COMPONENT emgucv_source
  FILES_MATCHING 
  PATTERN "*.cs"
  PATTERN "${CSPROJ_FILE}"
  PATTERN "*.resx"
  PATTERN ".svn" EXCLUDE
  PATTERN "obj" EXCLUDE
  PATTERN "CMakeFiles" EXCLUDE
  PATTERN "${PROJECT_NAME}.dir" EXCLUDE
  PATTERN "*Android*" EXCLUDE
  PATTERN "*MonoTouch*" EXCLUDE
)
ENDIF()  

ENDIF()
