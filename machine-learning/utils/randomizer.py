import os
import sys
import random
from PIL import Image
from xml.etree import ElementTree
from xml.etree.ElementTree import Element, SubElement, Comment
from xml.dom import minidom

def prettify(elem):
  """Return a pretty-printed XML string for the Element."""
  rough_string = ElementTree.tostring(elem, 'utf-8')
  reparsed = minidom.parseString(rough_string)
  return reparsed.toprettyxml(indent="  ")

def transPaste(background, foreground, box=(0, 0)):
    background = background.convert('RGBA')
    foregroundTrans = Image.new("RGBA", background.size)
    foregroundTrans.paste(foreground, box, mask=foreground)
    newImage = Image.alpha_composite(background, foregroundTrans)
    return newImage

def combineImage(option, path3D, pathBackGround, images3D, imagesBackGround, genDirPath, genDirName):
  imageName = 'combine1x' + str(option) + '_'
  for i in range(0, len(images3D), option):
    # Get a random background in background set
    backgroundIndex = random.randint(0, len(imagesBackGround)-1)
    background = Image.open(pathBackGround + '/' + imagesBackGround[backgroundIndex])
    # Get number of foreground based on user's option
    foregroundList = []
    if (option == 1):
      foregroundList.append(Image.open(path3D + '/' + images3D[i]))
    elif (option == 2):
      foregroundList.append(Image.open(path3D + '/' + images3D[i]))
      foregroundList.append(Image.open(path3D + '/' + images3D[i+1]))
    elif (option == 3):
      foregroundList.append(Image.open(path3D + '/' + images3D[i]))
      foregroundList.append(Image.open(path3D + '/' + images3D[i+1]))
      foregroundList.append(Image.open(path3D + '/' + images3D[i+2]))
    elif (option == 4):
      foregroundList.append(Image.open(path3D + '/' + images3D[i]))
      foregroundList.append(Image.open(path3D + '/' + images3D[i+1]))
      foregroundList.append(Image.open(path3D + '/' + images3D[i+2]))
      foregroundList.append(Image.open(path3D + '/' + images3D[i+3]))
    else:
      return 'Failed to generate images'
    # Get background size
    backgroundWidth, backgroundHeight = background.size
    # Random a position for foreground inside background
    foregroundPositionList = []
    occupiedZone = []
    for image in foregroundList:
      width, height = image.size
      check = True

      if (len(occupiedZone) <= 0):
        positionX = random.randint(0, backgroundWidth - width)
        positionY = random.randint(0, backgroundHeight - height)
        foregroundPositionList.append([positionX, positionY])
        occupiedZone.append([positionX, positionX+width, positionY, positionY+height])
      else:
        check = False
        retry = 100
        # Check occupied zone
        while (check == False and retry > 0):
          positionX = random.randint(0, backgroundWidth - width)
          positionY = random.randint(0, backgroundHeight - height)
          for j in range(len(occupiedZone)):
            if (
              (positionX+width >= occupiedZone[j][0] and positionX <= occupiedZone[j][1])
              or (positionY+height >= occupiedZone[j][2] and positionY <= occupiedZone[j][3])
            ):
              check = False
              break
            else:
              check = True
          if (check == True):
            foregroundPositionList.append([positionX, positionY])
            occupiedZone.append([positionX, positionX+width, positionY, positionY+height])
          retry = retry - 1
        if (retry == 0):
          foregroundPositionList.append([10000, 10000])
    # Combine foreground and background to a new image
    for j in range(len(foregroundList)):
      print(str(foregroundPositionList[j][0]) + ', ' + str(foregroundPositionList[j][1]))
      if (foregroundPositionList[j][0] == 10000):
        continue
      if (os.path.exists(genDirPath + '/' + genDirName + '/' + imageName + str(i) + '.png') == False):
        combinationImage = transPaste(background, foregroundList[j], box=(foregroundPositionList[j][0], foregroundPositionList[j][1]))
        combinationImagePath = genDirPath + '/' + genDirName + '/' + imageName + str(i) + '.png'
        combinationImage.save(combinationImagePath)
      else:
        newBackGround = Image.open(genDirPath + '/' + genDirName + '/' + imageName + str(i) + '.png')
        combinationImage = transPaste(newBackGround, foregroundList[j], box=(foregroundPositionList[j][0], foregroundPositionList[j][1]))
        combinationImagePath = genDirPath + '/' + genDirName + '/' + imageName + str(i) + '.png'
        combinationImage.save(combinationImagePath)
    # Parse new image information to XML file
    parseXML(foregroundList, imageName + str(i) + '.png', foregroundPositionList, genDirPath, genDirName)

def parseXML(foregroundList, combinationImageName, foregroundPositionList, genDirPath, genDirName):
  # Read size of image
  foregroundSizeList = []
  for image in foregroundList:
    width, height = image.size
    foregroundSizeList.append([width, height])
  
  # Parse image name
  imageArr = combinationImageName.split('.')

  # Create XML file structure
  top = Element('annotation')

  folder = SubElement(top, 'folder')
  folder.text = genDirName

  filename = SubElement(top, 'filename')
  filename.text = combinationImageName

  path = SubElement(top, 'path')
  path.text = genDirPath + '/' + genDirName + '/' + combinationImageName

  source = SubElement(top, 'source')
  subSource = SubElement(source, 'database')
  subSource.text = ''

  size = SubElement(top, 'size')
  widthSize = SubElement(size, 'width')
  widthSize.text = str(width)
  heightSize = SubElement(size, 'height')
  heightSize.text = str(height)
  depthSize = SubElement(size, 'depth')
  depthSize.text = '3'

  segmented = SubElement(top, 'segmented')
  segmented.text = '0'

  for i in range(len(foregroundList)):
    objectChild = SubElement(top, 'object')
    nameObject = SubElement(objectChild, 'name')
    nameObject.text = 'lego'
    poseObject = SubElement(objectChild, 'pose')
    poseObject.text = 'Unspecified'
    truncatedObject = SubElement(objectChild, 'truncated')
    truncatedObject.text = '0'
    difficultObject = SubElement(objectChild, 'difficult')
    difficultObject.text = '0'
    bndboxObject = SubElement(objectChild, 'bndbox')
    bndboxXMin = SubElement(bndboxObject, 'xmin')
    bndboxXMin.text = str(foregroundPositionList[i][0])
    bndboxYMin = SubElement(bndboxObject, 'ymin')
    bndboxYMin.text = str(foregroundPositionList[i][1])
    bndboxXMax = SubElement(bndboxObject, 'xmax')
    bndboxXMax.text = str(foregroundPositionList[i][0] + foregroundSizeList[i][0])
    bndboxYMax = SubElement(bndboxObject, 'ymax')
    bndboxYMax.text = str(foregroundPositionList[i][1] + foregroundSizeList[i][1])

  # Write XML structure to file
  f = open(genDirPath + '/' + genDirName + '/' + imageArr[0] + '.xml', 'w+')
  f.write(prettify(top))
  f.close()


def main():
  path3D = input('Enter the source folder of 3D models: ')
  pathBackGround = input('Enter the source folder of background: ')
  images3D = os.listdir(path3D)
  imagesBackGround = os.listdir(pathBackGround)
  genDirPath = input('Enter where you want to create the folder that contains artifacts: ')
  genDirName = input('Name the folder: ')
  option = input('Number of lego pieces: ')

  if (os.path.isdir(genDirPath + '/' + genDirName) == False):
    os.mkdir(genDirPath + '/' + genDirName)

  combineImage(int(option), path3D, pathBackGround, images3D, imagesBackGround, genDirPath, genDirName)

if __name__ == '__main__':
  main()