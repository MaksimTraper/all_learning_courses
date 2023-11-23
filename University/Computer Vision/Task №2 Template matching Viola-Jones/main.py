import cv2  
import numpy as np

desired_width = 280  # желаемая ширина
desired_height = 320  # желаемая высота
dim = (desired_width, desired_height)  # размер в итоге

# Загрузка исходного изображения
img = cv2.imread('C:/a/faces/2_1.jpg')
# Преобразуем в полутоновое изображение
gray_img = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
# Загрузка эталонного изображения
template = cv2.imread('C:/a/faces/1_1F.jpg', cv2.IMREAD_GRAYSCALE)
# Сохраняем ширину шаблона в w и высоту в h 
w, h = template.shape[::-1]  
# Сопоставляем
res = cv2.matchTemplate(gray_img,template,cv2.TM_CCOEFF_NORMED)  
# Пороговое значение  
threshold = 0.6
# Сохраняем координаты совпавшей области
loc = np.where(res >= threshold)  
# Рисуем прямоугольник вокруг совпавшей области.  
for pt in zip(*loc[::-1]):  
 cv2.rectangle(img, pt,(pt[0] + w, pt[1] + h),(0,255,255), 1)
# Меняем масштаб изображения
img = cv2.resize(img, dim)
# Выводим финальное изображение
cv2.imshow('Detected',img)
cv2.waitKey(0)