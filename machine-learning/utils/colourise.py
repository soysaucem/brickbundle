import pandas as pd
from PIL import Image
import numpy as np
import colorsys
import glob
import sys, os
import random

def load_colors_from_csv(csvFile):
    data = pd.read_csv(csvFile)
    return data

rgb_to_hsv = np.vectorize(colorsys.rgb_to_hsv)
hsv_to_rgb = np.vectorize(colorsys.hsv_to_rgb)

def shift_hue(arr, hout, sout):
    r, g, b, a = np.rollaxis(arr, axis=-1)
    h, s, v = rgb_to_hsv(r, g, b)
    s = sout
    h = hout
    r, g, b = hsv_to_rgb(h, s, v)
    arr = np.dstack((r, g, b, a))
    return arr

def colourise(image, hue, sat):
    img = image.convert('RGBA')
    arr = np.array(np.asarray(img).astype('float'))
    new_img = Image.fromarray(shift_hue(arr, hue/360., sat/360.).astype('uint8'), 'RGBA')

    return new_img

def random_resize(src, dest):
    for filename in glob.glob(src + '*.png'):
        im = Image.open(filename)
        size = random.randint(50,199)
        im.thumbnail((size,size), Image.ANTIALIAS)
        im.save(dest + os.path.basename(im.filename) + '_' + 'resized' + '.png')

def main():
    src = sys.argv[1]
    dest = sys.argv[2]   
    colors_csv = load_colors_from_csv("./lego_colors.csv");
    for filename in glob.glob(src + '*.png'):
        im = Image.open(filename)
        for index, row in colors_csv.iterrows():
            h, s, v = rgb_to_hsv(colors_csv.R[index], colors_csv.G[index], colors_csv.B[index])
            im2 = colourise(im, h*360., s*360.)
            im2.save(dest + os.path.basename(im.filename) + '_' + str(index) + '.png')
    os.mkdir(dest + 'resized')
    random_resize(dest, dest + 'resized/');
main()


