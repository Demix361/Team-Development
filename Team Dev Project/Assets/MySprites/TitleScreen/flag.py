import cv2, math

img = cv2.imread('flag.png', cv2.IMREAD_UNCHANGED)


h, w, _ = img.shape
h_offset = 60
w = 1509
y_offset = 0

for j in range(12):
	
	img_copy = img.copy()
	s = 0

	for i in range(w - 1, -1, -1):
		#x = round(math.sin((j * 40.25 + i) * 0.013) * 20)
		x = round(math.sin((j * 40.25 + i) * 0.013) * (1 + (w + 1 - i) * 0.03))

		if (i == w - 1 and j == 0):
			y_offset = x
		elif (j != 0 and i == w - 1):
			s = y_offset - x
		#x += s

		a = img_copy[h_offset : h - h_offset, i : i + 1]
		img_copy[h_offset + x : h - h_offset + x, i : i + 1] = a

	cv2.imwrite('new_flag' + str(j) + '.png', img_copy)