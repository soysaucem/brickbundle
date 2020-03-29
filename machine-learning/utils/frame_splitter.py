#!/usr/bin/env python
# coding: utf-8

# In[1]:


import cv2
import numpy as np
import os
from parsers.frame_splitter_parser import frame_splitter_parser


# In[2]:


fps = 25
args = frame_splitter_parser.parse_args()
video_path = args.video
folder, video_name = os.path.split(video_path)
cap = cv2.VideoCapture(video_path)
cap.set(cv2.CAP_PROP_FPS, fps)
currentFrame = 0
index = 0
curDir = os.getcwd()

if not os.path.isdir(curDir + '\\data\\'):
    os.mkdir(curDir + '\\data\\')
if not os.path.isdir(curDir + '\\data\\' + '\\frames\\'):
    os.mkdir(curDir + '\\data\\' + '\\frames\\')
if not os.path.isdir(curDir + '\\data\\' + '\\frames\\' + '\\' + video_name + '\\'):
    os.mkdir(curDir + '\\data\\' + '\\frames\\' + '\\' + video_name + '\\')

print(curDir)
print ('Running...')
while(True):
    retval, frame = cap.read()
    if not retval: break
    if currentFrame % 10 == 0:
        name = curDir + '/data/frames/' + video_name + '/' + video_name + '_frame_' + str(index) + '.jpg'
        cv2.imwrite(name, frame)
        index += 1
    currentFrame += 1
print ('Done, ' + str(index) + ' images created')
cap.release()
cv2.destroyAllWindows()
