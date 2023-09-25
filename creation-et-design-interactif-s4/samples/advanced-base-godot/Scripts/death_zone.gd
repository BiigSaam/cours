extends Area2D

func _on_body_entered(body):
	if body.is_in_group("player"):
		body.hit(1)
		body.position = body.last_checkpoint
	pass # Replace with function body.
