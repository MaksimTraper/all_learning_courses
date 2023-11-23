import os
import random
from tkinter import filedialog as fd
from os.path import dirname as up

import numpy
from matplotlib import pyplot as plt
from skimage.metrics import structural_similarity as ssim
import cv2
import numpy as np

class AutoGenScetches:
    p1 = np.array([])
    p2 = np.array([])
    p3 = np.array([])
    p_arr = [p1, p2, p3]
    amountScetches = 0
    intervalRandom = 0
    photo = None
    grPhoto = None
    scetch = None
    vScetch = None
    grScetch = None
    inverted_image = None
    generation1 = []
    generation2 = []
    ssim_gen1 = []
    ssim_gen2 = []

    def __init__(self, amountScetches, intervalRandom):
        self.amountScetches = amountScetches
        self.intervalRandom = intervalRandom
        return

    def makeViewedScetch(self):
        newPh = cv2.blur(self.photo, (10, 10))
        newPh = [n.astype('int') for n in newPh]
        newPh = self.photo - newPh
        dv = self.photo + self.photo
        for z in range(0,3):
            for y in range(0, 200):
                for i in range(0, 250):
                    if(dv[i,y,z] > 255):
                        dv[i,y,z] == 255
        newPh = abs(255 - newPh)
        newPh += dv
        newPh //= 2
        vScetch = newPh
        cv2.imshow('fe', vScetch)
        return

    def loadPhotoPath(self):
        photoPath = fd.askopenfilename()
        root_proj = os.getcwd()
        rel_path = os.path.relpath(photoPath, root_proj)
        photoPath = rel_path
        rel_path = up(rel_path)
        photo = os.path.relpath(photoPath, rel_path)
        scetchPath = 'sketches/' + photo[0].upper() + '2' + photo[1:8:1] + '-sz1.jpg'
        return photoPath, scetchPath

    def LoadPhotoAndScetch(self):
        photoPath, scetchPath = self.loadPhotoPath()
        self.photo = cv2.imread(photoPath)
        self.grPhoto = self.cvt_to_gray(self.photo)
        self.scetch = cv2.imread(scetchPath)
        self.grScetch = self.cvt_to_gray(self.scetch)
        return

    def cvt_to_gray(self, photo):
        gr_photo = cv2.cvtColor(photo, cv2.COLOR_BGR2GRAY)
        return gr_photo

    def GenerateP(self):
        arr = [[], [], []]
        for i in range(0, 3):
            for y in range(0, self.amountScetches):
                num = 0
                while(num == 0):
                    num = random.randint(-(self.intervalRandom), self.intervalRandom)
                arr[i] = np.append(arr[i], num)
        self.p1 = arr[0]
        self.p2 = arr[1]
        self.p3 = arr[2]
        return None

    def MakeFirstScetchGeneration(self):
        l = len(self.p1)
        for i in range (0, l):
            if(self.p1[i] > 0):
                a = np.delete(self.grScetch, np.s_[0:int(self.p1[i])-1], 0)
                self.generation1.append(a)
            else:
                endSlice = int(abs(self.p1[i]))-1
                sl = self.grScetch[0:endSlice]
                a = np.insert(self.grScetch, 0, sl, 0)
                self.generation1.append(a)
            self.generation1[i] = cv2.resize(self.generation1[i], (200, 250), cv2.COLOR_BGR2GRAY, cv2.INTER_LINEAR)
            fir = self.generation1[i]

            if (self.p2[i] > 0):
                border = int(self.p2[i])-1
                self.generation1[i] = np.delete(self.generation1[i], np.s_[0:border], 1)
            else:
                border = int(abs(self.p2[i]))+1
                r_border = 200 - border
                self.generation1[i] = np.delete(self.generation1[i], np.s_[r_border:200], 1)
            self.generation1[i] = cv2.resize(self.generation1[i], (200, 250), cv2.COLOR_BGR2GRAY)
            sec = self.generation1[i]

            if (self.p3[i] > 0):
                numForRoll = int(abs(self.p3[i])-1)
                arr1 = self.generation1[i][0:250, numForRoll:]
                arr2 = self.generation1[i][0:250, :numForRoll]
                self.generation1[i] = np.concatenate((arr1, arr2), axis=1)
            else:
                self.generation1[i] = np.roll(self.generation1[i], int(abs(self.p3[i])-1))
        return None

    def MakeSecondScetchGeneration(self):
        l = len(self.p1)
        for i in range(0, l):
            self.generation2.append(self.grScetch)
        self.generation2 = [n.astype('int') for n in self.generation2]
        for i in range(0, l):
            for y in range(0, i+1):
                self.generation2[i] += self.generation1[y]
            self.generation2[i] = self.generation2[i]/(i+2)
        self.generation2 = [n.round() for n in self.generation2]
        return None

    def CalculateSSIM(self):
        l = len(self.p1)
        self.generation2 = [n.astype('uint8') for n in self.generation2]
        global ssimor
        ssimor = ssim(self.grScetch, self.grPhoto)
        for i in range(0, l):
            self.ssim_gen1.append(ssim(self.generation1[i], self.grPhoto))
            self.ssim_gen2.append(ssim(self.generation2[i], self.grPhoto))
        return None

    def ShowResults(self):
        l = len(self.p1)
        numbers = list(range(1, l+1))
        s = []
        for i in range(0, l):
            s.append(ssimor)
        fig2 = plt.figure(layout="constrained")
        for i in range(1, 11):
            ax = fig2.add_subplot(2, 5, i)
            ax.set_title('№' + str(i) + ': ' + str(self.p1[i-1]) + ', ' + str(self.p2[i-1]) + ', ' + str(self.p3[i-1]))
            ax.imshow(self.generation1[i-1], cmap='gray')
            ax.axis('off')
        fig3 = plt.figure(layout="constrained")
        for i in range(1, 11):
            ax2 = fig3.add_subplot(2, 5, i)
            ax2.set_title('№' + str(i))
            ax2.imshow(self.generation2[i-1], cmap='gray')
            ax2.axis('off')
        fig4 = plt.figure()
        ax3 = fig4.add_subplot(1, 3, 1)
        ax3.set_title('Исходное фото')
        ax3.imshow(cv2.cvtColor(self.photo, cv2.COLOR_BGR2RGB))
        ax3.axis('off')
        ax3 = fig4.add_subplot(1, 3, 2)
        ax3.set_title('Artist scetch')
        ax3.imshow(self.scetch)
        ax3.axis('off')
        ax3 = fig4.add_subplot(1, 3, 3)
        ax3.plot(numbers, self.ssim_gen1, color="green", label='П1')
        ax3.plot(numbers, self.ssim_gen2, color="blue", label='П2')
        ax3.plot(numbers, s, color="red")
        ax3.legend()
        plt.xlabel("№ скетча в популяции")
        plt.ylabel("SSIM")
        plt.grid()
        fig2.canvas.manager.set_window_title('Генерация 1')
        fig3.canvas.manager.set_window_title('Генерация 2')
        fig4.canvas.manager.set_window_title('Итоговые результаты')
        fig2.show(), fig3.show(), fig4.show()
        plt.show()
        return None