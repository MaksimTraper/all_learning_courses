# импорт библиотек
import os

import cv2
import numpy as np
import matplotlib.pyplot as plt
import tkinter as tk
from tkinter import filedialog as fd
import tkinter.messagebox as mb
import operator
from skimage import io, measure, transform, metrics
from skimage.measure import block_reduce
import time
import fawkes
from os.path import dirname as up

def delay():
    sec = float(timedelay3.get())
    time.sleep(sec)
    return

# функция Гистограммы
def Bar_chart_func(file):
    # вычисление гистограммы
    histg = cv2.calcHist([file], [0], None, [256], [0, 256])
    return histg


# функция DFT
def DFT_func(file):
    # применение DFT (двумерное дискретное преобразование Фурье)
    dft = cv2.dft(np.float32(file), flags=cv2.DFT_COMPLEX_OUTPUT)
    # применение частотного (циклического) сдвига
    dft_shift = np.fft.fftshift(dft)
    # вычисление двухмерных векторов DFT
    magnitude_spectrum = 20 * np.log(cv2.magnitude(dft_shift[:, :, 0], dft_shift[:, :, 1]))
    return magnitude_spectrum


# функция DCT
def DCT_func(file):
    # применение DCT (двумерное дискретное косинусное преобразование)
    dct = cv2.dct(np.float32(file))
    return dct


# функция Scale
def Scale_func(file):
    # поиск изображения через путь
    img = io.imread(file)
    # изменение размера и вывод как отдельное изображение
    img_res = transform.resize(img, (20, 20))
    return img_res


# функция Градиента
def Gradient_func(file):
    # ввод величин смещения
    ksize = 3
    dx = 1
    dy = 1
    # вычисление градиента
    gradient_x = cv2.Sobel(file, cv2.CV_32F, dx, 0, ksize=ksize)
    gradient_y = cv2.Sobel(file, cv2.CV_32F, 0, dy, ksize=ksize)
    # преобразование значений градиента в абсолютные
    abs_gradient_x = cv2.convertScaleAbs(gradient_x)
    abs_gradient_y = cv2.convertScaleAbs(gradient_y)
    # подсчет итогового значения градиента
    gradient = cv2.addWeighted(abs_gradient_x, 0.5, abs_gradient_y, 0.5, 0)
    # суммирование значений
    SumGrad = []
    for i in range(0, len(gradient), 1):
        SumGrad.append(round(sum(gradient[i]) / len(gradient[i]), 1))
    return SumGrad


# функция построения графиков
def Charts_build_func(num_e, start_pos, step, intermed_pos, ch_func):
    ind_et = 0
    ind_t = 0
    # стат. массивы
    stat_dct = []
    stat_dft = []
    stat_scale = []
    stat_hist = []
    stat_grad = []
    # лимиты по разницам
    delta_k_h = int(delta1out.get())
    delta_k_g = int(delta2out.get())
    # массивы тестов
    t_img_a = []
    t_hist = []
    t_grad = []
    t_dft = []
    t_dct = []
    t_scale = []
    # массивы эталонов
    e_img_a = []
    e_hist = []
    e_grad = []
    e_dft = []
    e_dct = []
    e_scale = []

    stat_dct_indv = []
    stat_dft_indv = []
    stat_scale_indv = []
    stat_hist_indv = []
    stat_grad_indv = []

    if(ch_func == 0):
        help_pos = num_e
    if(ch_func == 1):
        help_pos = 1
        help_var = 0
    if(ch_func == 2):
        help_pos = 0
        help_var = 1
    # цикл
    #проход по папкам
    for i in range(1, 11, 1):
        sum_h = 0
        sum_g = 0
        sum_sim_dft = 0
        sum_sim_dct = 0
        sum_sim_scale = 0
        #проход по эталонам
        for j in range(start_pos, intermed_pos + 1, step):
            # начальные суммы результатов
            res_h = 0
            res_g = 0
            # эталоны
            fln_e = f"s{i}/{j}.pgm"
            e_img = cv2.imread(fln_e, cv2.IMREAD_GRAYSCALE)
            e_img_a.append(e_img)
            e_hist.append(Bar_chart_func(e_img))
            e_grad.append(Gradient_func(e_img))
            e_dft.append(DFT_func(e_img))
            e_dct.append(DCT_func(e_img))
            e_scale.append(Scale_func(fln_e))
            # Проход по тестам. Внутренний цикл - уровень 1
            for k in range(help_pos + 1, 11, step):
                # тесты
                fln_t = f"S{i}/{k}.pgm"
                t_img = cv2.imread(fln_t, cv2.IMREAD_GRAYSCALE)
                t_img_a.append(t_img)
                t_hist.append(Bar_chart_func(t_img))
                t_grad.append(Gradient_func(t_img))
                t_dft.append(DFT_func(t_img))
                t_dct.append(DCT_func(t_img))
                t_scale.append(Scale_func(fln_t))
                # индексы и максимумы через генераторы
                in_e_h, e_m_h = max(enumerate(e_hist[ind_et]), key=operator.itemgetter(1))
                in_e_g, e_m_g = max(enumerate(e_grad[ind_et]), key=operator.itemgetter(1))
                t_max_h = t_hist[ind_t][in_e_h]
                t_max_g = t_grad[ind_t][in_e_g]
                # вычисление разницы
                delt_h = abs(e_m_h - t_max_h)
                delt_g = abs(e_m_g - t_max_g)
                # проверка условий и добавление в счетчик
                if (delt_h < delta_k_h):
                    res_h += 1
                    stat_hist_indv.append(1)
                else:
                    stat_hist_indv.append(0)

                if (delt_g < delta_k_g):
                    stat_grad_indv.append(1)
                    res_g += 1
                else:
                    stat_grad_indv.append(0)
                # процент DFT
                mean_mag_e = np.mean(e_dft[ind_et])
                mean_mag_t = np.mean(t_dft[ind_t])
                similarity_percent_dft = mean_mag_t / mean_mag_e
                if (similarity_percent_dft > 1):
                    similarity_percent_dft = 2 - similarity_percent_dft
                sum_sim_dft += similarity_percent_dft
                # процент DCT
                linalg_norm_e = np.linalg.norm(e_dct[ind_et])
                linalg_norm_t = np.linalg.norm(t_dct[ind_t])
                similarity_percent_dct = linalg_norm_t / linalg_norm_e
                if (similarity_percent_dct > 1):
                    similarity_percent_dct = 2 - similarity_percent_dct
                sum_sim_dct += similarity_percent_dct
                ssim = metrics.structural_similarity(e_scale[ind_et],
                                                         t_scale[ind_t],
                                                         data_range=255)
                if (ssim > 1):
                    ssim = 2 - ssim
                sum_sim_scale += ssim
                stat_dft_indv.append(similarity_percent_dft)
                stat_dct_indv.append(similarity_percent_dct)
                stat_scale_indv.append(ssim)
                ind_t += 1
            # изменение сумм гистограммы и градиента
            sum_h += res_h
            sum_g += res_g
            ind_et += 1
        # добавление в статы
        stat_hist.append(sum_h / ((10 - num_e) * num_e))
        stat_grad.append(sum_g / ((10 - num_e) * num_e))
        stat_dft.append(sum_sim_dft / ((10 - num_e) * num_e))
        stat_dct.append(sum_sim_dct / ((10 - num_e) * num_e))
        stat_scale.append(sum_sim_scale / ((10 - num_e) * num_e))

    #Новые массивы, которые будут содержать результаты тестовых для каждой s
    stat_hist_indv_n = np.zeros((10-num_e) * 10)
    stat_grad_indv_n = np.zeros((10-num_e) * 10)
    stat_dft_indv_n = np.zeros((10-num_e) * 10)
    stat_dct_indv_n = np.zeros((10-num_e) * 10)
    stat_scale_indv_n = np.zeros((10-num_e) * 10)
    #Просмотреть каждый s
    for m in range(1, 10 + 1, 1):
        ad = 0
        for l in range(0+(m-1)*(10-num_e),(10-num_e)*m,1):
            for o in range(0, num_e,1):
                v = ad + (10-num_e) * o + (m-1) * num_e * (10-num_e)
                # Идея этого процесса заключается в расчете среднего показателя тестовых относительно кол-ва эталонов
                stat_hist_indv_n[l] += stat_hist_indv[v]
                stat_grad_indv_n[l] += stat_grad_indv[v]
                stat_dft_indv_n[l] += stat_dft_indv[v]
                stat_dct_indv_n[l] += stat_dct_indv[v]
                stat_scale_indv_n[l] += stat_scale_indv[v]
            ad += 1
            stat_hist_indv_n[l] = stat_hist_indv_n[l] / num_e
            stat_grad_indv_n[l] = stat_grad_indv_n[l] / num_e
            stat_dft_indv_n[l] = stat_dft_indv_n[l] / num_e
            stat_dct_indv_n[l] = stat_dct_indv_n[l] / num_e
            stat_scale_indv_n[l] = stat_scale_indv_n[l] / num_e
    # построение окон через фигуры
    # окно 1
    figure1, ((ax_1, ax_2, ax_3, ax_4, ax_5, ax_6), (ax1, ax2, ax3, ax4, ax5, ax6)) = plt.subplots(2, 6)
    # окно 2
    figure2, (axH, axG, axDFT, axDCT, axScale) = plt.subplots(1, 5)
    #окно 3
    figure3, (bxH, bxG, bxDFT, bxDCT, bxScale) = plt.subplots(1, 5)
    plt.ion()
    # корректировка окна 1
    # тест
    ax_1.set_title('Тест')
    i_a = ax_1.imshow(t_img_a[0], cmap="gray")
    ax_2.set_title('Гистограмма')
    h_a, = ax_2.plot(t_hist[0], color="yellow")
    ax_3.set_title('DFT')
    df_a = ax_3.imshow(t_dft[0], cmap='gray', vmin=0, vmax=255)
    ax_4.set_title('DCT')
    dc_a = ax_4.imshow(np.abs(t_dct[0]), vmin=0, vmax=255)
    x = np.arange(len(t_grad[0]))
    ax_5.set_title('Градиент')
    g_a, = ax_5.plot(x, t_grad[0], color="yellow")
    ax_6.set_title('Scale')
    sc_a = ax_6.imshow(t_scale[0], cmap="gray")
    # эталон
    ax1.set_title('Эталон')
    i_a_e = ax1.imshow(e_img_a[0], cmap="gray")
    ax2.set_title('Гистограмма')
    h_a_e, = ax2.plot(e_hist[0], color="yellow")
    ax3.set_title('DFT')
    df_a_e = ax3.imshow(e_dft[0], cmap='gray', vmin=0, vmax=255)
    ax4.set_title('DCT')
    dc_a_e = ax4.imshow(np.abs(e_dct[0]), vmin=0, vmax=255)
    x_e = np.arange(len(e_grad[0]))
    ax5.set_title('Градиент')
    g_a_e, = ax5.plot(x_e, e_grad[0], color="yellow")
    ax6.set_title('Scale')

    sc_a_e = ax6.imshow(e_scale[0], cmap="gray")
    x_r_g = np.arange(1, len(stat_grad) + 1)
    x_r_h = np.arange(1, len(stat_hist) + 1)
    x_r_dft = np.arange(1, len(stat_dft) + 1)
    x_r_dct = np.arange(1, len(stat_dct) + 1)
    x_r_scale = np.arange(1, len(stat_scale) + 1)

    x_r_g_ind = np.arange(1, len(stat_grad_indv_n) + 1)
    x_r_h_ind = np.arange(1, len(stat_hist_indv_n) + 1)
    x_r_dft_ind = np.arange(1, len(stat_dft_indv_n) + 1)
    x_r_dct_ind = np.arange(1, len(stat_dct_indv_n) + 1)
    x_r_scale_ind = np.arange(1, len(stat_scale_indv_n) + 1)
    # статы
    # показ окон
    figure1.canvas.manager.set_window_title('Эксперимент')
    figure2.canvas.manager.set_window_title('Результаты экспериментов по тестовым')
    figure3.canvas.manager.set_window_title('Результаты экспериментов по классам')
    figure1.show()
    figure2.show()
    figure3.show()
    # внутренний цикл - уровень 1
    for t in range(0, 10, 1):
        # корректировка окна 2
        bxH.plot(x_r_h[0:t+1:1], stat_hist[0:t+1:1], color="yellow")
        bxH.set_title('Hist')
        bxG.plot(x_r_g[0:t+1:1], stat_grad[0:t+1:1], color="yellow")
        bxG.set_title('Grad')
        bxDFT.plot(x_r_dft[0:t+1:1], stat_dft[0:t+1:1], color="yellow")
        bxDFT.set_title('DFT')
        bxDCT.plot(x_r_dct[0:t+1:1], stat_dct[0:t+1:1], color="yellow")
        bxDCT.set_title('DCT')
        bxScale.plot(x_r_scale[0:t+1:1], stat_scale[0:t+1:1], color="yellow")
        bxScale.set_title('Scale')
        # внутренний цикл - уровень 2
        for p in range(num_e * t, num_e * t + num_e, 1):
            # запись значений на график - эталоны
            i_a_e.set_data(e_img_a[p])
            h_a_e.set_ydata(e_hist[p])
            df_a_e.set_data(e_dft[p])
            dc_a_e.set_data(e_dct[p])
            g_a_e.set_ydata(e_grad[p])
            sc_a_e.set_data(e_scale[p])
            # внутренний цикл - уровень 3
            for m in range((p * (10 - num_e)), (10 - num_e) * (p + 1), 1):
                # запись значений на график - тесты
                i_a.set_data(t_img_a[m])
                h_a.set_ydata(t_hist[m])
                df_a.set_data(t_dft[m])
                dc_a.set_data(t_dct[m])
                g_a.set_ydata(t_grad[m])
                sc_a.set_data(t_scale[m])

                axH.set_title('Hist')
                axG.set_title('Grad')
                axDFT.set_title('DFT')
                axDCT.set_title('DCT')
                axScale.set_title('Scale')
                if(m%((10-num_e)*num_e)) >= ((10-num_e) * (num_e-1)):
                    axH.plot(x_r_h_ind[0:m%((10-num_e)*(t+1)) + 1:1], stat_hist_indv_n[0:m%((10-num_e)*(t+1)) + 1:1],
                                color="b")
                    axG.plot(x_r_g_ind[0:m%((10-num_e)*(t+1)) + 1:1], stat_grad_indv_n[0:m%((10-num_e)*(t+1)) + 1:1],
                                color="b")
                    axDFT.plot(x_r_dft_ind[0:m%((10-num_e)*(t+1)) + 1:1], stat_dft_indv_n[0:m%((10-num_e)*(t+1)) + 1:1],
                                  color="b")
                    axDCT.plot(x_r_dct_ind[0:m%((10-num_e)*(t+1)) + 1:1], stat_dct_indv_n[0:m%((10-num_e)*(t+1)) + 1:1],
                                  color="b")
                    axScale.plot(x_r_scale_ind[0:m%((10-num_e)*(t+1)) + 1:1], stat_scale_indv_n[0:m%((10-num_e)*(t+1)) + 1:1],
                                    color="b")

                # зарисовка окна 1
                figure1.canvas.draw()
                figure1.canvas.flush_events()

                delay()


# функция ввода числа эталонов через поле ввода
def GetE_func(choosed_fun):
    num_etalons = num_etalons_entry.get()

    if(choosed_fun == 0):#эталоны берутся по порядку
        start_pos = 1
        step = 1
        intermed_pos = int(num_etalons)
    if(choosed_fun == 1):#эталоны нечётные
        start_pos = 1
        step = 2
        intermed_pos = 10
        num_etalons = str(5)
    if(choosed_fun == 2):#эталоны чётные
        start_pos = 2
        step = 2
        intermed_pos = 10
        num_etalons = str(5)

    if num_etalons.isdigit() and int(num_etalons) > 0:
        Charts_build_func(int(num_etalons), start_pos, step, intermed_pos, choosed_fun)
    else:
        tk.showerror("Ошибка!", "Должно быть введено целое положительное число!")

def Cmp_choosed_photos():
    filename1 = fd.askopenfilename()
    filename2 = fd.askopenfilename()
    Charts_Selected_func(filename1, filename2)

# функция выбора изображений для сравнения
def Cmp_masked_photo():
    command = 'fawkes_binary_windows-v1.0 -d .\\'
    filename1 = fd.askopenfilename()
    root_proj = os.getcwd()
    rel_path = os.path.relpath(filename1, root_proj)
    rel_path = up(rel_path)
    d = up(up(filename1))
    d = d + "/"
    filename1 = filename1.replace(d, '')
    command = command + rel_path + ' -m low'
    os.system(command)
    cloaked = filename1
    cloaked = cloaked.replace('.jpg', '_low_cloaked.png')
    cloaked = cloaked.replace('.pgm', '_low_cloaked.png')
    # применение функции сравнения двух файлов
    Charts_Selected_func(filename1, cloaked)

# функция показа результатов
def ResultsShow_func(text):
    msg = text
    mb.showinfo("Результат", msg)


# функция сравнения произвольно выбранных файлов
def Charts_Selected_func(filename1, filename2):
    # лимиты разниц
    delta_k_h = int(delta1out.get())
    delta_k_g = int(delta2out.get())

    # начальные результаты гистограммы и градиента
    res_h = 0
    res_g = 0

    # эталон (первый файл)
    e_img = cv2.imread(filename1, cv2.IMREAD_GRAYSCALE)
    e_hist = Bar_chart_func(e_img)
    e_grad = Gradient_func(e_img)
    e_dft = DFT_func(e_img)
    e_dct = DCT_func(e_img)
    e_scale = Scale_func(filename1)

    # тест (второй файл)
    t_img = cv2.imread(filename2, cv2.IMREAD_GRAYSCALE)
    t_hist = Bar_chart_func(t_img)
    t_grad = Gradient_func(t_img)
    t_dft = DFT_func(t_img)
    t_dct = DCT_func(t_img)
    t_scale = Scale_func(filename2)

    # чтение теста и эталона через pyplot
    t_or_img = plt.imread(filename2, cv2.IMREAD_GRAYSCALE)
    e_or_img = plt.imread(filename1, cv2.IMREAD_GRAYSCALE)

    # индексы через генераторы
    in_e_m, e_m_h = max(enumerate(e_hist), key=operator.itemgetter(1))
    in_e_g, e_m_g = max(enumerate(e_grad), key=operator.itemgetter(1))

    # максимумы
    t_max_h = t_hist[in_e_m]
    t_max_g = t_grad[in_e_g]

    # вычисление разниц
    delt_c = abs(e_m_h - t_max_h)
    delt_g = abs(e_m_g - t_max_g)
    if (delt_c < delta_k_h):
        res_h += 1
    if (delt_g < delta_k_g):
        res_g += 1

    # построение графиков
    # построение графика - эталон
    plt.subplot(3, 6, 13)
    plt.imshow(e_or_img, cmap='gray')
    plt.title("Эталон")

    plt.subplot(3, 6, 14)
    plt.plot(e_hist, color="yellow")
    plt.title("Гистограмма")

    plt.subplot(3, 6, 15)
    plt.imshow(e_dft, cmap='gray', vmin=0, vmax=255)
    plt.title("DFT")

    plt.subplot(3, 6, 16)
    plt.imshow(np.abs(e_dct), vmin=0, vmax=255)
    plt.title("DCT")

    plt.subplot(3, 6, 17)
    x = np.arange(len(e_grad))
    plt.plot(x, e_grad, color="yellow")
    plt.title("Градиент")

    plt.subplot(3, 6, 18)
    plt.imshow(e_scale, cmap='gray')
    plt.title("Scale")

    # построение графика - тест
    plt.subplot(3, 6, 1)
    plt.imshow(t_or_img)
    plt.title("Тест")

    plt.subplot(3, 6, 2)
    plt.plot(t_hist, color="yellow")
    plt.title("Гистограмма")

    plt.subplot(3, 6, 3)
    plt.imshow(t_dft, cmap='gray', vmin=0, vmax=255)
    plt.title("DFT")

    plt.subplot(3, 6, 4)
    plt.imshow(np.abs(t_dct), vmin=0, vmax=255)
    plt.title("DCT")

    plt.subplot(3, 6, 5)
    x = np.arange(len(t_grad))
    plt.plot(x, t_grad, color="yellow")
    plt.title("Градиент")

    plt.subplot(3, 6, 6)
    plt.imshow(t_scale, cmap='gray')
    plt.title("Scale")

    # вывод результата
    if (res_g != 0 and res_h != 0):
        ResultsShow_func("Совпадение есть!")
    else:
        ResultsShow_func("Нет совпадения!")
    plt.show()


# активация основных функций
# главное окно
root = tk.Tk()
root.title("Программа для моделирования систем распознавания людей по лицам")
root.geometry("300x370")
# поле для ввода кол-ва эталонов
num_etalons_label = tk.Label(root, text="Количество эталонов:")
num_etalons_label.pack()
num_etalons_entry = tk.Entry(root)
num_etalons_entry.insert(0, '3')
num_etalons_entry.pack()

# кнопка активации - по циклу БД
plot_button = tk.Button(root, text="Выполнить", command= lambda :GetE_func(int(0)))
plot_button.pack()

# кнопка активации - произвольные
select_files_label = tk.Label(root, text="Произвольное сравнение:")
select_files_label.pack()
plot_button = tk.Button(root, text="Выбрать и выполнить", command=Cmp_choosed_photos)
plot_button.pack()

select_files_label = tk.Label(root, text="Маскировка и сравнение фотографии:")
select_files_label.pack()
plot_button = tk.Button(root, text="Выбрать и выполнить", command=Cmp_masked_photo)
plot_button.pack()

cross_val_lab = tk.Label(root, text="Кросс-Валидация")
cross_val_lab.pack()
plot_button2 = tk.Button(root, text="Нечетные эталоны", command= lambda :GetE_func(int(1)))
plot_button2.pack()
plot_button3 = tk.Button(root, text="Четные эталоны", command= lambda :GetE_func(int(2)))
plot_button3.pack()

delta1 = tk.Label(root, text='Пороговое значение гистограммы')
delta1.pack()
delta1out = tk.Entry(root)
delta1out.insert(0, '250') #значение по умолчанию
delta1out.pack()

delta2 = tk.Label(root, text='Пороговое значение градиента')
delta2.pack()
delta2out = tk.Entry(root)
delta2out.insert(0, '80') #значение по умолчанию
delta2out.pack()

timedelay = tk.Label(root, text='Задержка между шагами')
timedelay.pack()
timedelay3 = tk.Entry(root)
timedelay3.insert(0, '1') #значение по умолчанию
timedelay3.pack()
# главный цикл обработки событий
root.mainloop()